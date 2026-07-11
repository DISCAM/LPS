using Data.Dtos.WarehouseOrders;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IWarehouseOrdersService
    {
        Task<List<WarehouseOrderDto>> GetAllAsync();

        Task<WarehouseOrderDetailsDto> GetByIdAsync(int id);

        Task<WarehouseOrderDetailsDto> CreateAsync(
            CreateWarehouseOrderDto dto,
            string identityUserId);

        Task<ShipLogisticUnitResultDto> ShipLogisticUnitAsync(
            int warehouseOrderId,
            ShipLogisticUnitDto dto,
            string identityUserId);
    }
}
