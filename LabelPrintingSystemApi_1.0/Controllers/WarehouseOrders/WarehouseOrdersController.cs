using Data;
using Data.Dtos.WarehouseOrders;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabelPrintingSystemApi_1._0.Controllers.WarehouseOrders
{
    [ApiController]
    [Authorize]
    public class WarehouseOrdersController : ControllerBase
    {
        private readonly IWarehouseOrdersService warehouseOrdersService;

        public WarehouseOrdersController(IWarehouseOrdersService warehouseOrdersService)
        {
            this.warehouseOrdersService = warehouseOrdersService;
        }

        [HttpGet]
        [Route(Urls.WAREHOUSE_ORDERS)]
        public async Task<IActionResult> GetAllWarehouseOrdersAsync()
        {
            List<WarehouseOrderDto> result = await this.warehouseOrdersService.GetAllAsync();

            return this.Ok(result);
        }

        [HttpGet]
        [Route(Urls.WAREHOUSE_ORDERS_ID)]
        public async Task<IActionResult> GetWarehouseOrderByIdAsync([FromRoute] int id)
        {
            WarehouseOrderDetailsDto result = await this.warehouseOrdersService.GetByIdAsync(id);

            return this.Ok(result);
        }

        [HttpPost]
        [Route(Urls.WAREHOUSE_ORDERS)]
        public async Task<IActionResult> CreateWarehouseOrderAsync([FromBody] CreateWarehouseOrderDto dto)
        {
            string? identityUserId = this.User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.Unauthorized();
            }

            WarehouseOrderDetailsDto result =
                await this.warehouseOrdersService.CreateAsync(dto, identityUserId);

            return this.StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost]
        [Route(Urls.WAREHOUSE_ORDERS_SHIP_LOGISTIC_UNIT)]
        public async Task<IActionResult> ShipLogisticUnitAsync(
            [FromRoute] int id,
            [FromBody] ShipLogisticUnitDto dto
        )
        {
            string? identityUserId = this.User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.Unauthorized();
            }

            ShipLogisticUnitResultDto result =
                await this.warehouseOrdersService.ShipLogisticUnitAsync(
                    id,
                    dto,
                    identityUserId
                );

            return this.Ok(result);
        }
    }
}