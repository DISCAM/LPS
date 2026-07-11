using Data;
using Data.Dtos.Dashboard;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.Dashboard
{
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        [HttpGet]
        [Route(Urls.DASHBOARD_SUMMARY)]
        public async Task<IActionResult> GetDashboardSummaryAsync()
        {
            DashboardSummaryDto result =
                await this.dashboardService.GetSummaryAsync();

            return this.Ok(result);
        }
    }
}