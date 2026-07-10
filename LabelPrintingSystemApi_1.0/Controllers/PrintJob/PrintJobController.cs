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

        //[HttpGet]
        //[Route(Urls.PRINT_JOBS_PREVIEW)]
        //public async Task<IActionResult> GetPrintJobPreviewByIdAsync([FromRoute] int id)
        //{
        //    string previewFilePath =
        //        await printJobPreviewDispatcher.GetPreviewPathAsync(id);
        //
        //    return PhysicalFile(
        //        previewFilePath,
        //        "image/png"
        //    );
        //}

        [HttpGet]
        [Route(Urls.PRINT_JOBS_PREVIEW)]
        public async Task<IActionResult> GetPrintJobPreviewByIdAsync([FromRoute] int id)
        {
            string previewFilePath =
                await this.printJobPreviewDispatcher.GetPreviewPathAsync(id);

            if (!System.IO.File.Exists(previewFilePath))
            {
                return this.NotFound("Nie znaleziono pliku podglądu etykiety.");
            }

            byte[] previewBytes = await ReadPreviewFileBytesAsync(previewFilePath);

            this.Response.Headers["Cache-Control"] =
                "no-store, no-cache, must-revalidate, max-age=0";

            this.Response.Headers["Pragma"] = "no-cache";

            this.Response.Headers["Expires"] = "0";

            return this.File(previewBytes, "image/png");
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

        private static async Task<byte[]> ReadPreviewFileBytesAsync(string previewFilePath)
        {
            const int maxAttempts = 40;
            const int delayMilliseconds = 250;

            Exception? lastException = null;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    if (!System.IO.File.Exists(previewFilePath))
                    {
                        await Task.Delay(delayMilliseconds);
                        continue;
                    }

                    System.IO.FileInfo fileInfo = new(previewFilePath);

                    if (fileInfo.Length == 0)
                    {
                        await Task.Delay(delayMilliseconds);
                        continue;
                    }

                    await using System.IO.FileStream fileStream = new(
                        previewFilePath,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read,
                        System.IO.FileShare.ReadWrite
                    );

                    using MemoryStream memoryStream = new();

                    await fileStream.CopyToAsync(memoryStream);

                    byte[] previewBytes = memoryStream.ToArray();

                    if (previewBytes.Length > 0)
                    {
                        return previewBytes;
                    }
                }
                catch (System.IO.IOException exception)
                {
                    lastException = exception;

                    await Task.Delay(delayMilliseconds);
                }
                catch (UnauthorizedAccessException exception)
                {
                    lastException = exception;

                    await Task.Delay(delayMilliseconds);
                }
            }

            throw new System.IO.IOException(
                "Nie udało się odczytać pliku podglądu etykiety, " +
                "ponieważ nadal jest używany przez inny proces.",
                lastException
            );
        }



    }
}