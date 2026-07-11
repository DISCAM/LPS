using Data.Dtos.Dashboard;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
