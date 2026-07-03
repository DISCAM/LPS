using Data;
using Data.Dtos.PrintJob;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabelPrintingSystemApi_1._0.Controllers.PrintJobs
{
    [ApiController]
    [Authorize]
    public class PrintJobController : ControllerBase
    {
        private readonly IPrintJobService printJobService;
        private readonly IPrintJobPreviewDispatcher printJobPreviewDispatcher;

        public PrintJobController(IPrintJobService printJobService, IPrintJobPreviewDispatcher printJobPreviewDispatcher)
        {
            this.printJobService = printJobService;
            this.printJobPreviewDispatcher = printJobPreviewDispatcher;
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

        [HttpGet]
        [Route(Urls.PRINT_JOBS_PREVIEW)]
        public async Task<IActionResult> GetPrintJobPreviewByIdAsync([FromRoute] int id)
        {
            string previewFilePath =
                await printJobPreviewDispatcher.GetPreviewPathAsync(id);

            return PhysicalFile(
                previewFilePath,
                "image/png"
            );
        }


        [HttpPatch]
        [Route(Urls.PRINT_JOBS_CANCEL)]
        public async Task<IActionResult> PrintJobIdCancelAsync([FromRoute] int id)
        {
            string? identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return Unauthorized();
            }

            await printJobService.CancelPrintJobAsync(id, identityUserId);

            return NoContent();

        }

        [HttpPost]
        [Route(Urls.PRINT_JOBS_REPRINT)]
        public async Task<IActionResult> ReprintPrintJobByIdAsync([FromRoute] int id)
        {
            string? identityUserId = User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return Unauthorized();
            }

            ReprintPrintJobResultDto result =
                await printJobService.ReprintPrintJobAsync(
                    id,
                    identityUserId
                );

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost]
        [Route(Urls.PRINT_JOBS_PRINT)]
        public async Task<IActionResult> ExecutePrintJobByIdAsync([FromRoute] int id)
        {
            string? identityUserId = User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return Unauthorized();
            }

            await printJobService.ExecutePrintJobAsync(
                id,
                identityUserId
            );

            // Od razu zwracamy aktualny stan po wysłaniu do NiceLabel:
            // SENT albo ERROR.
            PrintJobDetailsDto updatedPrintJob =
                await printJobService.GetPrintJobByIdAsync(id);

            return Ok(updatedPrintJob);
        }







    }
}