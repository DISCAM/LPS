using Data.Dtos.PrintJob;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IPrintJobService
    {
        Task<List<PrintJobListDto>> GetAllPrintJobsAsync();
        Task<PrintJobDetailsDto> GetPrintJobByIdAsync(int printJobId);
        Task CancelPrintJobAsync(int printJobId, string identityUserId);

        Task<ReprintPrintJobResultDto> ReprintPrintJobAsync(int printJobId, string identityUserId);
    }
}