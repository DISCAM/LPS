using Data;
using Data.Dtos.PrintLabel;
using LabelPrintingSystemApi_1._0.Controllers;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


[ApiController]
[Authorize]
public class PrintLabelController : ControllerBase
{
    private readonly IPrintLabelService printLabelService;

    public PrintLabelController(IPrintLabelService printLabelService)
    {
        this.printLabelService = printLabelService;
    }

    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    [HttpPost]
    [Route(Urls.PRINT_EAN)]
    public async Task<IActionResult> PrintEanAsync([FromBody] PrintEanDto dto)
    {
        string? identityUserId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return Unauthorized();
        }

        PrintResultDto result = await printLabelService.PrintEanAsync(
            dto,
            identityUserId
        );

        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    [HttpPost]
    [Route(Urls.PRINT_PRODUCTION_LABEL)]
    public async Task<IActionResult> PrintProductionLabelAsync([FromBody] PrintProductionLabelDto dto)
    {
        string? identityUserId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return Unauthorized();
        }

        PrintResultDto result =
            await printLabelService.PrintProductionLabelAsync(dto, identityUserId);

        return Ok(result);
    }
}
