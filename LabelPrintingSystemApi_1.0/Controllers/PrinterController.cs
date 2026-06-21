using Data;
using Data.Dtos.Printer;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers
{
    [ApiController]
    [Authorize]
    public class PrinterController : ControllerBase
    {
        private readonly IPrinterService printerService;

        public PrinterController(IPrinterService printerService)
        {
            this.printerService = printerService;
        }

        [HttpGet]
        [Route(Urls.PRINTERS)]
        public async Task<IActionResult> GetAllPrintersAsync()
        {
            IEnumerable<PrinterDto> printers =
                await printerService.GetAllPrintersAsync();

            return Ok(printers);
        }

        [HttpGet]
        [Route(Urls.PRINTER_ID)]
        public async Task<IActionResult> GetPrinterByIdAsync(int id)
        {
            PrinterDto printer = await printerService.GetPrinterByIdAsync(id);

            return Ok(printer);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [Route(Urls.PRINTER_ADD)]
        public async Task<IActionResult> CreatePrinterAsync(
            [FromBody] PrinterCreateDto dto
        )
        {
            await printerService.CreatePrinterAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut]
        [Route(Urls.PRINTER_EDIT)]
        public async Task<IActionResult> EditPrinterAsync(
            int id,
            [FromBody] PrinterEditDto dto
        )
        {
            if (id != dto.PrinterId)
            {
                return BadRequest("ID mismatch");
            }

            await printerService.EditPrinterAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete]
        [Route(Urls.PRINTER_DELETE)]
        public async Task<IActionResult> DeletePrinterAsync(int id)
        {
            await printerService.DeletePrinterAsync(id);

            return Ok();
        }
    }
}