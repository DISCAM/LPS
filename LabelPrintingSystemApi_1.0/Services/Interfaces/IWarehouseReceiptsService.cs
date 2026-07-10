using Data.Dtos.WarehouseReceipts;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IWarehouseReceiptsService
    {
        Task<WarehouseReceiptResultDto> CreateAsync(CreateWarehouseReceiptDto dto, string identityUserId);
    }
}
