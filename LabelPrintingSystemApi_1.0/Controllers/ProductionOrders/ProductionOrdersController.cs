using Data;
using Data.Dtos.ProductionOrders;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabelPrintingSystemApi_1._0.Controllers.ProductionOrders
{
    [ApiController]
    [Authorize]
    public class ProductionOrdersController : ControllerBase
    {
        private readonly IProductionOrdersService productionOrdersService;

        public ProductionOrdersController(
            IProductionOrdersService productionOrdersService)
        {
            this.productionOrdersService = productionOrdersService;
        }

        [HttpGet]
        [Route(Urls.PRODUCTION_ORDERS)]
        public async Task<ActionResult<List<ProductionOrderDto>>> GetAll()
        {
            var productionOrders =
                await this.productionOrdersService.GetAllAsync();

            return this.Ok(productionOrders);
        }

        [HttpPost]
        [Route(Urls.PRODUCTION_ORDERS)]
        public async Task<ActionResult<ProductionOrderDto>> Create(
            [FromBody] CreateProductionOrderDto dto)
        {
            var identityUserId = this.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.Unauthorized(
                    "Nie udało się odczytać zalogowanego użytkownika.");
            }

            var productionOrder =
                await this.productionOrdersService.CreateAsync(
                    dto,
                    identityUserId);

            return this.Ok(productionOrder);
        }
    }
}