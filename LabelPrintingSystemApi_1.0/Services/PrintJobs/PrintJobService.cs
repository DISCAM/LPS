using Data.Dtos.PrintJob;
using Data.Dtos.PrintLabel;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LabelPrintingSystemApi_1._0.Services.PrintJobs
{
    public class PrintJobService : IPrintJobService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<PrintJobService> logger;

        private static readonly JsonSerializerOptions labelDataJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PrintJobService(
            DatabaseContext databaseContext,
            ILogger<PrintJobService> logger
        )
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
        }

        public async Task<List<PrintJobListDto>> GetAllPrintJobsAsync()
        {
            List<PrintJobListDto> printJobs = await databaseContext.PrintJobs
                .AsNoTracking()
                .OrderByDescending((printJob) => printJob.CreatedAt)
                .Select((printJob) => new PrintJobListDto
                {
                    PrintJobId = printJob.PrintJobId,
                    LabelId = printJob.LabelId,

                    LabelType = printJob.Label.LabelType,

                    ProductName = printJob.Label.Product != null
                        ? printJob.Label.Product.Name
                        : null,

                    PrimaryCodeValue = printJob.Label.PrimaryCodeValue,

                    TemplateName = printJob.Label.LabelTemplate.Name,

                    PrinterName = printJob.Printer.Name,

                    Copies = printJob.Copies,

                    Status = printJob.Status,

                    IsReprint = printJob.IsReprint,

                    CreatedByUserName = printJob.CreatedByUser.FullName,

                    CreatedAt = printJob.CreatedAt,

                    ErrorMessage = printJob.ErrorMessage,
                })
                .ToListAsync();

            logger.LogInformation(
                "Retrieved {PrintJobCount} print jobs.",
                printJobs.Count
            );

            return printJobs;
        }

        public async Task<PrintJobDetailsDto> GetPrintJobByIdAsync(int printJobId)
        {
            var result = await databaseContext.PrintJobs
                .AsNoTracking()
                .Where(printJob => printJob.PrintJobId == printJobId)
                .Select(printJob => new
                {
                    LabelDataJson = printJob.Label.LabelDataJson,

                    Details = new PrintJobDetailsDto
                    {
                        PrintJobId = printJob.PrintJobId,
                        LabelId = printJob.LabelId,

                        LabelType = printJob.Label.LabelType,

                        ProductCode = printJob.Label.Product != null
                            ? printJob.Label.Product.ProductCode
                            : null,

                        ProductName = printJob.Label.Product != null
                            ? printJob.Label.Product.Name
                            : null,

                        PrimaryCodeValue = printJob.Label.PrimaryCodeValue,

                        TemplateName = printJob.Label.LabelTemplate.Name,

                        PrinterName = printJob.Printer.Name,

                        Copies = printJob.Copies,

                        Status = printJob.Status,

                        IsReprint = printJob.IsReprint,

                        ErrorMessage = printJob.ErrorMessage,

                        CreatedByUserName = printJob.CreatedByUser.FullName,

                        CreatedAt = printJob.CreatedAt,

                        ModifiedByUserName = printJob.ModifiedByUser != null
                            ? printJob.ModifiedByUser.FullName
                            : null,

                        ModifiedAt = printJob.ModifiedAt,

                        History = printJob.PrintJobHistories
                            .OrderByDescending(history => history.CreatedAt)
                            .Select(history => new PrintJobHistoryDto
                            {
                                CreatedAt = history.CreatedAt,
                                Status = history.Status,
                                ErrorMessage = history.ErrorMessage,
                                Note = history.Note,
                            })
                            .ToList(),
                    }
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );
            }

            PrintEanLabelDataDto? labelData =
                DeserializeLabelData(result.LabelDataJson);

            result.Details.LabelData = labelData;

            logger.LogInformation(
                "Pobrano szczegóły zadania wydruku o ID {PrintJobId}.",
                printJobId
            );

            return result.Details;
        }

        public async Task CancelPrintJobAsync(int printJobId, string identityUserId)
        {
            User currentUser = await databaseContext.Users
                .FirstOrDefaultAsync(user =>
                user.IdentityUserId == identityUserId &&
                user.IsActive)
                ?? throw new NotFoundException("Nie znaleziono aktywnego użytkownika.");

            PrintJob printJob = await databaseContext.PrintJobs
                .FirstOrDefaultAsync(job => job.PrintJobId == printJobId)
                ?? throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );

            if (printJob.Status != "QUEUED")
            {
                throw new BadRequestException(
                    "Można anulować wyłącznie zadanie wydruku ze statusem QUEUED."
                );
            }

            DateTime cancelledAt = DateTime.UtcNow;

            printJob.Status = "CANCELLED";
            printJob.ModifiedByUserId = currentUser.UserId;
            printJob.ModifiedAt = cancelledAt;

            PrintJobHistory history = new PrintJobHistory
            {
                PrintJobId = printJob.PrintJobId,
                Status = "CANCELLED",
                CreatedAt = cancelledAt,
                Note = "Zadanie wydruku anulowane przez użytkownika."
            };

            databaseContext.PrintJobHistories.Add(history);

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                "Print job {PrintJobId} was cancelled by user {UserId}.",
                printJob.PrintJobId,
                currentUser.UserId
            );
        }

        public async Task<ReprintPrintJobResultDto> ReprintPrintJobAsync(int printJobId, string identityUserId)
        {
            User currentUser = await databaseContext.Users
                .FirstOrDefaultAsync(user =>
                    user.IdentityUserId == identityUserId &&
                    user.IsActive)
                ?? throw new NotFoundException(
                    "Nie znaleziono aktywnego użytkownika."
                );

            PrintJob sourcePrintJob = await databaseContext.PrintJobs
                .Include(printJob => printJob.Printer)
                .FirstOrDefaultAsync(printJob =>
                    printJob.PrintJobId == printJobId)
                ?? throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );

            bool canBeReprinted =
                sourcePrintJob.Status == "PRINTED" ||
                sourcePrintJob.Status == "ERROR" ||
                sourcePrintJob.Status == "CANCELLED";

            if (!canBeReprinted)
            {
                throw new BadRequestException(
                    "Reprint można utworzyć wyłącznie dla zadania " +
                    "ze statusem PRINTED, ERROR lub CANCELLED."
                );
            }

            if (!sourcePrintJob.Printer.IsActive)
            {
                throw new BadRequestException(
                    "Nie można utworzyć reprintu, ponieważ drukarka " +
                    "przypisana do pierwotnego zadania jest nieaktywna."
                );
            }

            DateTime reprintCreatedAt = DateTime.UtcNow;

            await using var transaction =
                await databaseContext.Database.BeginTransactionAsync();

            try
            {
                PrintJob reprintPrintJob = new PrintJob
                {
                    LabelId = sourcePrintJob.LabelId,

                    PrinterId = sourcePrintJob.PrinterId,

                    CreatedByUserId = currentUser.UserId,

                    Copies = sourcePrintJob.Copies,

                    Status = "QUEUED",

                    IsReprint = true,

                    CreatedAt = reprintCreatedAt,
                };

                databaseContext.PrintJobs.Add(reprintPrintJob);

                await databaseContext.SaveChangesAsync();

                PrintJobHistory history = new PrintJobHistory
                {
                    PrintJobId = reprintPrintJob.PrintJobId,

                    Status = "QUEUED",

                    CreatedAt = reprintCreatedAt,

                    Note =
                        $"Utworzono reprint na podstawie zadania wydruku " +
                        $"o ID {sourcePrintJob.PrintJobId}.",
                };

                databaseContext.PrintJobHistories.Add(history);

                await databaseContext.SaveChangesAsync();

                await transaction.CommitAsync();

                logger.LogInformation(
                    "Reprint print job {ReprintPrintJobId} was created " +
                    "from print job {SourcePrintJobId} by user {UserId}.",
                    reprintPrintJob.PrintJobId,
                    sourcePrintJob.PrintJobId,
                    currentUser.UserId
                );

                return new ReprintPrintJobResultDto
                {
                    PrintJobId = reprintPrintJob.PrintJobId,

                    LabelId = reprintPrintJob.LabelId,

                    Status = reprintPrintJob.Status,

                    IsReprint = reprintPrintJob.IsReprint,

                    CreatedAt = reprintPrintJob.CreatedAt,
                };
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }


        private PrintEanLabelDataDto? DeserializeLabelData(string? labelDataJson)
        {
            if (string.IsNullOrWhiteSpace(labelDataJson))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<PrintEanLabelDataDto>(
                    labelDataJson,
                    labelDataJsonOptions
                );
            }
            catch (JsonException exception)
            {
                logger.LogWarning(
                    exception,
                    "Could not deserialize LabelDataJson."
                );

                return null;
            }
        }
    }
}