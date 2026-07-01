using Data.Dtos.Dispatchers;
using Data.Dtos.PrintLabel;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http.Json;

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

        public NiceLabelPrintJobDispatcher(
            DatabaseContext databaseContext,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<NiceLabelPrintJobDispatcher> logger
        )
        {
            this.databaseContext = databaseContext;
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task DispatchPrintJobAsync(int printJobId)
        {
            PrintJob printJob = await databaseContext.PrintJobs
                .Include(item => item.Label)
                .Include(item => item.Printer)
                .FirstOrDefaultAsync(item =>
                    item.PrintJobId == printJobId)
                ?? throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );

            if (printJob.Status != "QUEUED")
            {
                throw new BadRequestException(
                    "Można przekazać wyłącznie zadanie wydruku ze statusem QUEUED."
                );
            }

            
            string? labelDataJson = printJob.Label.LabelDataJson;

            if (string.IsNullOrWhiteSpace(labelDataJson))
            {
                await MarkAsErrorAsync(
                    printJob,
                    "Brak danych snapshotu etykiety."
                );

                return;
            }

            PrintEanLabelDataDto? labelData =
                DeserializeLabelData(labelDataJson);

            if (labelData == null)
            {
                await MarkAsErrorAsync(
                    printJob,
                    "Nie udało się odczytać danych snapshotu etykiety."
                );

                return;
            }


            if (string.IsNullOrWhiteSpace(labelData.TemplateReference))
            {
                await MarkAsErrorAsync(
                    printJob,
                    "Brak TemplateReference w snapshotcie etykiety."
                );

                return;
            }

            string? triggerUrl =
                configuration["NiceLabelAutomation:PrintTriggerUrl"];

            if (string.IsNullOrWhiteSpace(triggerUrl))
            {
                await MarkAsErrorAsync(
                    printJob,
                    "Brak konfiguracji NiceLabelAutomation:PrintTriggerUrl."
                );

                return;
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
                IsReprint = printJob.IsReprint,

                ProductCode = labelData.ProductCode,
                ProductName = labelData.ProductName,
                Description = labelData.Description,
                Ean = labelData.Ean,
                Gtin = labelData.Gtin,
            };

            try
            {
                using HttpResponseMessage response =
                    await httpClient.PostAsJsonAsync(
                        triggerUrl,
                        dispatchDto
                    );

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    string errorMessage =
                        $"NiceLabel Automation zwrócił HTTP " +
                        $"{(int)response.StatusCode} " +
                        $"({response.ReasonPhrase}).";

                    if (!string.IsNullOrWhiteSpace(responseBody))
                    {
                        errorMessage += $" Odpowiedź: {responseBody}";
                    }

                    await MarkAsErrorAsync(
                        printJob,
                        errorMessage
                    );

                    return;
                }

                await MarkAsSentAsync(printJob);

                logger.LogInformation(
                    "Zadanie wydruku {PrintJobId} zostało przekazane do NiceLabel Automation.",
                    printJob.PrintJobId
                );
            }
            catch (HttpRequestException exception)
            {
                await MarkAsErrorAsync(
                    printJob,
                    $"Nie udało się połączyć z NiceLabel Automation: {exception.Message}"
                );

                logger.LogError(
                    exception,
                    "Błąd połączenia z NiceLabel Automation dla PrintJob {PrintJobId}.",
                    printJob.PrintJobId
                );
            }
            catch (TaskCanceledException exception)
            {
                await MarkAsErrorAsync(
                    printJob,
                    $"Przekroczono czas oczekiwania na odpowiedź NiceLabel Automation: {exception.Message}"
                );

                logger.LogError(
                    exception,
                    "Przekroczono czas oczekiwania dla PrintJob {PrintJobId}.",
                    printJob.PrintJobId
                );
            }
        }

        private PrintEanLabelDataDto? DeserializeLabelData(
            string labelDataJson
        )
        {
            try
            {
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

        private async Task MarkAsSentAsync(PrintJob printJob)
        {
            DateTime changedAt = DateTime.UtcNow;

            printJob.Status = "SENT";
            printJob.ErrorMessage = null;
            printJob.ModifiedAt = changedAt;

            PrintJobHistory history = new()
            {
                PrintJobId = printJob.PrintJobId,
                Status = "SENT",
                CreatedAt = changedAt,
                Note = "Zadanie przekazano do NiceLabel Automation.",
            };

            databaseContext.PrintJobHistories.Add(history);

            await databaseContext.SaveChangesAsync();
        }

        private async Task MarkAsErrorAsync(
            PrintJob printJob,
            string errorMessage
        )
        {
            DateTime changedAt = DateTime.UtcNow;

            printJob.Status = "ERROR";
            printJob.ErrorMessage = errorMessage;
            printJob.ModifiedAt = changedAt;

            PrintJobHistory history = new()
            {
                PrintJobId = printJob.PrintJobId,
                Status = "ERROR",
                ErrorMessage = errorMessage,
                CreatedAt = changedAt,
                Note = "Nie udało się przekazać zadania do NiceLabel Automation.",
            };

            databaseContext.PrintJobHistories.Add(history);

            await databaseContext.SaveChangesAsync();
        }
    }
}
