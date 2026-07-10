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
    public class NiceLabelPrintJobPreviewDispatcher : IPrintJobPreviewDispatcher
    {
        private static readonly JsonSerializerOptions labelDataJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly DatabaseContext databaseContext;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<NiceLabelPrintJobPreviewDispatcher> logger;

        public NiceLabelPrintJobPreviewDispatcher(
            DatabaseContext databaseContext,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<NiceLabelPrintJobPreviewDispatcher> logger
        )
        {
            this.databaseContext = databaseContext;
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<string> GetPreviewPathAsync(int printJobId)
        {
            string previewCacheDirectory =
                this.configuration["NiceLabelAutomation:PreviewCacheDirectory"]
                ?? throw new InvalidOperationException(
                    "Brak konfiguracji NiceLabelAutomation:PreviewCacheDirectory."
                );

            Directory.CreateDirectory(previewCacheDirectory);

            string previewFileName = $"print-job-{printJobId}.png";

            string previewFilePath = Path.Combine(
                previewCacheDirectory,
                previewFileName
            );

            if (File.Exists(previewFilePath))
            {
                this.logger.LogInformation(
                    "Zwrócono istniejący podgląd dla PrintJob {PrintJobId}.",
                    printJobId
                );

                return previewFilePath;
            }

            PrintJob printJob = await this.databaseContext.PrintJobs
                .AsNoTracking()
                .Include(item => item.Label)
                .FirstOrDefaultAsync(item =>
                    item.PrintJobId == printJobId
                )
                ?? throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );

            string? labelDataJson = printJob.Label.LabelDataJson;

            if (string.IsNullOrWhiteSpace(labelDataJson))
            {
                throw new BadRequestException(
                    "Brak danych snapshotu etykiety."
                );
            }

            PrintEanLabelDataDto? labelData = this.DeserializeLabelData(
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

            string? previewTriggerUrl =
                this.configuration["NiceLabelAutomation:PreviewTriggerUrl"];

            if (string.IsNullOrWhiteSpace(previewTriggerUrl))
            {
                throw new InvalidOperationException(
                    "Brak konfiguracji NiceLabelAutomation:PreviewTriggerUrl."
                );
            }

            PrintJobPreviewDispatchDto previewDispatchDto = new()
            {
                PrintJobId = printJob.PrintJobId,
                LabelId = printJob.LabelId,
                LabelType = printJob.Label.LabelType,

                TemplateReference = labelData.TemplateReference,
                TemplateVersionNo = labelData.TemplateVersionNo,

                PreviewFilePath = previewFilePath,

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
                previewDispatchDto.ProductionOrderNumber =
                    productionLabelData.ProductionOrderNumber;

                previewDispatchDto.LotNumber =
                    productionLabelData.LotNumber;

                previewDispatchDto.ProductionDate =
                    productionLabelData.ProductionDate;

                previewDispatchDto.ExpirationDate =
                    productionLabelData.ExpirationDate;

                previewDispatchDto.ProductionLine =
                    productionLabelData.ProductionLine;

                previewDispatchDto.ShiftCode =
                    productionLabelData.ShiftCode;

                previewDispatchDto.ProducedQuantity =
                    productionLabelData.ProducedQuantity;
            }

            if (
                printJob.Label.LabelType == "LOGISTIC" &&
                labelData is PrintLogisticLabelDataDto logisticLabelData
            )
            {
                previewDispatchDto.Sscc =
                    logisticLabelData.Sscc;

                previewDispatchDto.UnitType =
                    logisticLabelData.UnitType;

                previewDispatchDto.LotNumber =
                    logisticLabelData.LotNumber;

                previewDispatchDto.ProductionDate =
                    logisticLabelData.ProductionDate;

                previewDispatchDto.ExpirationDate =
                    logisticLabelData.ExpirationDate;

                previewDispatchDto.Quantity =
                    logisticLabelData.Quantity;
            }

            try
            {
                using HttpResponseMessage response =
                    await this.httpClient.PostAsJsonAsync(
                        previewTriggerUrl,
                        previewDispatchDto
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

                bool previewFileExists =
                    await WaitForPreviewFileAsync(previewFilePath);

                if (!previewFileExists)
                {
                    throw new InvalidOperationException(
                        "NiceLabel Automation zakończył działanie, " +
                        "ale plik podglądu nie został utworzony."
                    );
                }

                this.logger.LogInformation(
                    "Wygenerowano podgląd dla PrintJob {PrintJobId}.",
                    printJobId
                );

                return previewFilePath;
            }
            catch (HttpRequestException exception)
            {
                this.logger.LogError(
                    exception,
                    "Nie udało się połączyć z NiceLabel Automation " +
                    "podczas generowania preview dla PrintJob {PrintJobId}.",
                    printJobId
                );

                throw new InvalidOperationException(
                    "Nie udało się połączyć z NiceLabel Automation.",
                    exception
                );
            }
            catch (TaskCanceledException exception)
            {
                this.logger.LogError(
                    exception,
                    "Przekroczono czas oczekiwania na preview " +
                    "dla PrintJob {PrintJobId}.",
                    printJobId
                );

                throw new InvalidOperationException(
                    "Przekroczono czas oczekiwania na odpowiedź NiceLabel Automation.",
                    exception
                );
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
                this.logger.LogError(
                    exception,
                    "Nie udało się zdeserializować LabelDataJson."
                );

                return null;
            }
        }

        private static async Task<bool> WaitForPreviewFileAsync(
            string previewFilePath
        )
        {
            const int maxAttempts = 10;
            const int delayMilliseconds = 200;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                if (File.Exists(previewFilePath))
                {
                    return true;
                }

                await Task.Delay(delayMilliseconds);
            }

            return false;
        }
    }
}