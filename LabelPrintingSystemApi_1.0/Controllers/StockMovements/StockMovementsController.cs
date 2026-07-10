using Data;
using Data.Dtos.StockMovements;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.StockMovements
{
    [ApiController]
    [Authorize]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementsService stockMovementsService;

        public StockMovementsController(IStockMovementsService stockMovementsService)
        {
            this.stockMovementsService = stockMovementsService;
        }

        [HttpGet]
        [Route(Urls.STOCK_MOVEMENTS)]
        public async Task<ActionResult<List<StockMovementDto>>> GetAll()
        {
            List<StockMovementDto> stockMovements = await this.stockMovementsService.GetAllAsync();

            return this.Ok(stockMovements);
        }
    }
}