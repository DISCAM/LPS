using Data.Dtos.PrintJob;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.PrintJobs
{
    public class PrintJobService : IPrintJobService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<PrintJobService> logger;

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
            PrintJobDetailsDto? printJob = await databaseContext.PrintJobs
                .AsNoTracking()
                .Where((printJob) => printJob.PrintJobId == printJobId)
                .Select((printJob) => new PrintJobDetailsDto
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
                        .OrderByDescending((history) => history.CreatedAt)
                        .Select((history) => new PrintJobHistoryDto
                        {
                            CreatedAt = history.CreatedAt,
                            Status = history.Status,
                            ErrorMessage = history.ErrorMessage,
                            Note = history.Note,
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();

            if (printJob == null)
            {
                throw new NotFoundException(
                    $"Nie znaleziono zadania wydruku o ID {printJobId}."
                );
            }

            logger.LogInformation(
                "Pobrano szczegóły zadania wydruku o ID {PrintJobId}.",
                printJobId
            );

            return printJob;
        }


    }
}