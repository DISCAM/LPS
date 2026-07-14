using Data.Dtos.ProductionLots;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IProductionLotsService
    {
        Task<List<ProductionLotDto>> GetAllAsync();
        Task<List<ProductionLotDto>> GetAllByProductionOrderIdAsync(int productionOrderId);

        Task<ProductionLotDto> CreateAsync(int productionOrderId, CreateProductionLotDto dto, string identityUserId);
    }
}
