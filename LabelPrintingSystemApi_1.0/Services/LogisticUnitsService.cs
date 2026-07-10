using Data.Dtos.LogisticUnits;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.LogisticUnits
{
    public class LogisticUnitsService : ILogisticUnitsService
    {
        private readonly DatabaseContext databaseContext;

        public LogisticUnitsService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<LogisticUnitDto>> GetAllAsync()
        {
            return await this.databaseContext.LogisticUnits
                .AsNoTracking()
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new LogisticUnitDto
                {
                    LogisticUnitId = item.LogisticUnitId,
                    Sscc = item.Sscc,
                    UnitType = item.UnitType,
                    Status = item.Status,

                    TotalQuantity = item.LogisticUnitItems
                        .Sum(logisticUnitItem => logisticUnitItem.Quantity),

                    WarehouseOrderId = item.WarehouseOrderId,
                    WarehouseOrderNumber = item.WarehouseOrder != null
                        ? item.WarehouseOrder.OrderNumber
                        : null,

                    CreatedByUserId = item.CreatedByUserId,
                    CreatedByUserName = item.CreatedByUser.FullName,
                    CreatedAt = item.CreatedAt,

                    ModifiedByUserId = item.ModifiedByUserId,
                    ModifiedAt = item.ModifiedAt,

                    Items = item.LogisticUnitItems
                        .OrderBy(logisticUnitItem =>logisticUnitItem.LogisticUnitItemId)
                        .Select(logisticUnitItem => new LogisticUnitItemDto
                        {
                            LogisticUnitItemId = logisticUnitItem.LogisticUnitItemId,

                            ProductionLotId = logisticUnitItem.ProductionLotId,

                            LotNumber = logisticUnitItem.ProductionLot.LotNumber,

                            ProductionDate = logisticUnitItem.ProductionLot.ProductionDate,

                            ExpirationDate = logisticUnitItem.ProductionLot.ExpirationDate,

                            ProductId = logisticUnitItem.ProductionLot.ProductionOrder.ProductId,

                            ProductCode = logisticUnitItem.ProductionLot.ProductionOrder.Product.ProductCode,

                            ProductName = logisticUnitItem.ProductionLot.ProductionOrder.Product.Name,

                            Quantity = logisticUnitItem.Quantity,

                            WarehouseOrderItemId = logisticUnitItem.WarehouseOrderItemId,

                            CreatedAt = logisticUnitItem.CreatedAt,
                        }).ToList(),
                }).ToListAsync();
        }
    }
}