using System.Net.Http.Json;
using System.Text.Json;
using Data.Dtos.Dispatchers;
using Data.Dtos.PrintLabel;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.Dispatchers
{
    public class NiceLabelPrintJobDispatcher : IPrintJobDispatcher
    {
        private static readonly JsonSerializerOptions labelDataJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly DatabaseContext databaseContext;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<NiceLabelPrintJobDispatcher> logger;
        private readonly IAuditLogService auditLogService;

        public NiceLabelPrintJobDispatcher(
            DatabaseContext databaseContext,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<NiceLabelPrintJobDispatcher> logger,
            IAuditLogService auditLogService
        )
        {
            this.databaseContext = databaseContext;
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
            this.auditLogService = auditLogService;
        }

        public async Task DispatchPrintJobAsync(int printJobId)
        {
            PrintJob printJob = await databaseContext.PrintJobs
                .Include(item => item.Label)
                .Include(item => item.Printer)
                .FirstOrDefaultAsync(item =>
                    item.PrintJobId == printJobId
                )
                ?? throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );

            if (printJob.Status != "QUEUED")
            {
                throw new BadRequestException(
                    "Do NiceLabel można wysłać tylko zadanie o statusie QUEUED."
                );
            }

            string? labelDataJson = printJob.Label.LabelDataJson;

            if (string.IsNullOrWhiteSpace(labelDataJson))
            {
                throw new BadRequestException(
                    "Brak danych snapshotu etykiety."
                );
            }

            PrintEanLabelDataDto? labelData = DeserializeLabelData(
                printJob.Label.LabelType,
                labelDataJson
            );

            if (labelData == null)
            {
                throw new BadRequestException(
                    "Nie udało się odczytać danych snapshotu etykiety."
                );
            }

            if (string.IsNullOrWhiteSpace(labelData.TemplateReference))
            {
                throw new BadRequestException(
                    "Brak TemplateReference w snapshotcie etykiety."
                );
            }

            string? printTriggerUrl = configuration["NiceLabelAutomation:PrintTriggerUrl"];

            if (string.IsNullOrWhiteSpace(printTriggerUrl))
            {
                throw new InvalidOperationException(
                    "Brak konfiguracji NiceLabelAutomation:PrintTriggerUrl."
                );
            }

            PrintJobDispatchDto dispatchDto = new()
            {
                PrintJobId = printJob.PrintJobId,
                LabelId = printJob.LabelId,
                LabelType = printJob.Label.LabelType,

                TemplateReference = labelData.TemplateReference,
                TemplateVersionNo = labelData.TemplateVersionNo,

                PrinterName = printJob.Printer.Name,
                Copies = printJob.Copies,

                ProductCode = labelData.ProductCode,
                ProductName = labelData.ProductName,
                Description = labelData.Description,
                Ean = labelData.Ean,
                Gtin = labelData.Gtin,
            };

            if (
                printJob.Label.LabelType == "PRODUCTION" &&
                labelData is PrintProductionLabelDataDto productionLabelData
            )
            {
                dispatchDto.ProductionOrderNumber = productionLabelData.ProductionOrderNumber;
                dispatchDto.LotNumber = productionLabelData.LotNumber;
                dispatchDto.ProductionDate = productionLabelData.ProductionDate;
                dispatchDto.ExpirationDate = productionLabelData.ExpirationDate;
                dispatchDto.ProductionLine = productionLabelData.ProductionLine;
                dispatchDto.ShiftCode = productionLabelData.ShiftCode;
                dispatchDto.ProducedQuantity = productionLabelData.ProducedQuantity;
            }

            if (
                printJob.Label.LabelType == "LOGISTIC" &&
                labelData is PrintLogisticLabelDataDto logisticLabelData
            )
            {
                dispatchDto.Sscc = logisticLabelData.Sscc;
                dispatchDto.UnitType = logisticLabelData.UnitType;
                dispatchDto.LotNumber = logisticLabelData.LotNumber;
                dispatchDto.ProductionDate = logisticLabelData.ProductionDate;
                dispatchDto.ExpirationDate = logisticLabelData.ExpirationDate;
                dispatchDto.Quantity = logisticLabelData.Quantity;
            }

            try
            {
                using HttpResponseMessage response =
                    await httpClient.PostAsJsonAsync(
                        printTriggerUrl,
                        dispatchDto
                    );

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    throw new InvalidOperationException(
                        $"NiceLabel Automation zwrócił HTTP " +
                        $"{(int)response.StatusCode} " +
                        $"({response.ReasonPhrase}). " +
                        $"Odpowiedź: {responseBody}"
                    );
                }

                DateTime now = DateTime.Now;

                printJob.Status = "SENT";
                printJob.ErrorMessage = null;
                printJob.ModifiedAt = now;

                PrintJobHistory printJobHistory = new()
                {
                    PrintJobId = printJob.PrintJobId,
                    Status = printJob.Status,
                    Note = "Zadanie wydruku zostało wysłane do NiceLabel Automation.",
                    CreatedAt = now,
                };

                await databaseContext.PrintJobHistories.AddAsync(printJobHistory);

                await auditLogService.AddAsync(printJob.ModifiedByUserId ?? printJob.CreatedByUserId,
                    "PrintJob",
                    printJob.PrintJobId,
                    "SEND_PRINT_JOB",
                    $"Wysłano zadanie wydruku ID {printJob.PrintJobId} " +
                    $"do NiceLabel Automation. " +
                    $"Typ etykiety: {printJob.Label.LabelType}. " +
                    $"Drukarka: {printJob.Printer.Name}. " +
                    $"Liczba kopii: {printJob.Copies}. " +
                    $"Status: SENT.");

                await databaseContext.SaveChangesAsync();

                logger.LogInformation(
                    "Wysłano PrintJob {PrintJobId} do NiceLabel Automation.",
                    printJob.PrintJobId
                );
            }
            catch (HttpRequestException exception)
            {
                await MarkPrintJobAsErrorAsync(
                    printJob,
                    "Nie udało się połączyć z NiceLabel Automation.",
                    exception
                );

                throw new InvalidOperationException(
                    "Nie udało się połączyć z NiceLabel Automation.",
                    exception
                );
            }
            catch (TaskCanceledException exception)
            {
                await MarkPrintJobAsErrorAsync(
                    printJob,
                    "Przekroczono czas oczekiwania na odpowiedź NiceLabel Automation.",
                    exception
                );

                throw new InvalidOperationException(
                    "Przekroczono czas oczekiwania na odpowiedź NiceLabel Automation.",
                    exception
                );
            }
            catch (InvalidOperationException exception)
            {
                await MarkPrintJobAsErrorAsync(
                    printJob,
                    exception.Message,
                    exception
                );

                throw;
            }
        }

        private PrintEanLabelDataDto? DeserializeLabelData(
            string labelType,
            string labelDataJson
        )
        {
            try
            {
                if (labelType == "LOGISTIC")
                {
                    return JsonSerializer.Deserialize<PrintLogisticLabelDataDto>(
                        labelDataJson,
                        labelDataJsonOptions
                    );
                }

                if (labelType == "PRODUCTION")
                {
                    return JsonSerializer.Deserialize<PrintProductionLabelDataDto>(
                        labelDataJson,
                        labelDataJsonOptions
                    );
                }

                return JsonSerializer.Deserialize<PrintEanLabelDataDto>(
                    labelDataJson,
                    labelDataJsonOptions
                );
            }
            catch (JsonException exception)
            {
                logger.LogError(
                    exception,
                    "Nie udało się zdeserializować LabelDataJson."
                );

                return null;
            }
        }

        private async Task MarkPrintJobAsErrorAsync(
            PrintJob printJob,
            string errorMessage,
            Exception exception
        )
        {
            logger.LogError(
                exception,
                "Błąd podczas wysyłania PrintJob {PrintJobId} do NiceLabel Automation.",
                printJob.PrintJobId
            );

            DateTime now = DateTime.Now;

            printJob.Status = "ERROR";
            printJob.ErrorMessage = errorMessage;
            printJob.ModifiedAt = now;

            PrintJobHistory printJobHistory = new()
            {
                PrintJobId = printJob.PrintJobId,
                Status = printJob.Status,
                Note = "Wystąpił błąd podczas wysyłania zadania do NiceLabel Automation.",
                ErrorMessage = errorMessage,
                CreatedAt = now,
            };

            await databaseContext.PrintJobHistories.AddAsync(printJobHistory);

            await auditLogService.AddAsync( 
                printJob.ModifiedByUserId ?? printJob.CreatedByUserId,
                "PrintJob",
                printJob.PrintJobId,
                "PRINT_JOB_ERROR",
                $"Wystąpił błąd podczas wysyłania zadania wydruku " +
                $"ID {printJob.PrintJobId} do NiceLabel Automation. " +
                $"Typ etykiety: {printJob.Label.LabelType}. " +
                $"Drukarka: {printJob.Printer.Name}. " +
                $"Błąd: {errorMessage}");

            await databaseContext.SaveChangesAsync();
        }
    }
}