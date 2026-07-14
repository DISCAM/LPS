using Data;
using Data.Dtos.ProductionLots;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabelPrintingSystemApi_1._0.Controllers.ProductionLots
{
    [ApiController]
    [Authorize]
    public class ProductionLotsController : ControllerBase
    {
        private readonly IProductionLotsService productionLotsService;

        public ProductionLotsController(IProductionLotsService productionLotsService)
        {
            this.productionLotsService = productionLotsService;
        }


        [HttpGet]
        [Route(Urls.PRODUCTION_LOTS)]
        public async Task<ActionResult<List<ProductionLotDto>>> GetAll()
        {
            List<ProductionLotDto> productionLots = await this.productionLotsService.GetAllAsync();

            return this.Ok(productionLots);
        }

        [HttpGet]
        [Route(Urls.PRODUCTION_ORDER_LOTS)]
        public async Task<ActionResult<List<ProductionLotDto>>> GetAllByProductionOrderId([FromRoute] int productionOrderId)
        {
            var productionLots = await this.productionLotsService
                .GetAllByProductionOrderIdAsync(productionOrderId);

            return this.Ok(productionLots);
        }

        [HttpPost]
        [Route(Urls.PRODUCTION_ORDER_LOTS)]
        public async Task<ActionResult<ProductionLotDto>> Create([FromRoute] int productionOrderId, [FromBody] CreateProductionLotDto dto)
        {
            var identityUserId = this.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.Unauthorized(
                    "Nie udało się odczytać zalogowanego użytkownika.");
            }

            var productionLot = await this.productionLotsService.CreateAsync(productionOrderId, dto, identityUserId);

            return this.Ok(productionLot);
        }
    }
}
