using Data.Dtos.StockMovements;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IStockMovementsService
    {
        Task<List<StockMovementDto>> GetAllAsync();
    }
}
