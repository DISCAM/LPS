using Data;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.PrintJobs
{
    [ApiController]
    [Authorize]
    public class PrintJobController : ControllerBase
    {
        private readonly IPrintJobService printJobService;

        public PrintJobController(IPrintJobService printJobService)
        {
            this.printJobService = printJobService;
        }

        [HttpGet]
        [Route(Urls.PRINT_JOBS)]
        public async Task<IActionResult> GetAllPrintJobsAsync()
        {
            var printJobs = await printJobService.GetAllPrintJobsAsync();

            return Ok(printJobs);
        }

        [HttpGet]
        [Route(Urls.PRINT_JOBS_ID)]
        public async Task<IActionResult> GetPrintJobByIdAsync([FromRoute] int id)
        {
            var printJob = await printJobService.GetPrintJobByIdAsync(id);

            return Ok(printJob);
        }
    }
}