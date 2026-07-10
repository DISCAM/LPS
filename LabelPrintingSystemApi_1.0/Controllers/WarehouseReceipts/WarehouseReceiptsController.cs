using System.Security.Claims;
using Data;
using Data.Dtos.WarehouseReceipts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.WarehouseReceipts
{
    [ApiController]
    [Authorize]
    public class WarehouseReceiptsController : ControllerBase
    {
        private readonly IWarehouseReceiptsService warehouseReceiptsService;

        public WarehouseReceiptsController(
            IWarehouseReceiptsService warehouseReceiptsService
        )
        {
            this.warehouseReceiptsService = warehouseReceiptsService;
        }

        [HttpPost]
        [Route(Urls.WAREHOUSE_RECEIPTS)]
        public async Task<ActionResult<WarehouseReceiptResultDto>> Create(
            [FromBody] CreateWarehouseReceiptDto dto
        )
        {
            string? identityUserId =
                this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.Unauthorized();
            }

            WarehouseReceiptResultDto result =
                await this.warehouseReceiptsService.CreateAsync(
                    dto,
                    identityUserId
                );

            return this.Ok(result);
        }
    }
}