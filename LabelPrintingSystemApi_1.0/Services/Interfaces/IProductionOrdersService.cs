using Data.Dtos.ProductionOrders;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IProductionOrdersService
    {
        Task<ProductionOrderDto> CreateAsync(
            CreateProductionOrderDto dto,
            string identityUserId);
        Task<List<ProductionOrderDto>> GetAllAsync();
    }
}