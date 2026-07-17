using Data.Dtos.StockMovements;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.StockMovements
{
    public class StockMovementsService : IStockMovementsService
    {
        private readonly DatabaseContext databaseContext;

        public StockMovementsService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<StockMovementDto>> GetAllAsync()
        {
            return await this.databaseContext.StockMovements
                .AsNoTracking()
                .Include(item => item.ProductionLot)
                .ThenInclude(item => item.ProductionOrder)
                .ThenInclude(item => item.Product)
                .Include(item => item.LogisticUnit)
                .Include(item => item.WarehouseOrder)
                .Include(item => item.CreatedByUser)
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new StockMovementDto
                {
                    StockMovementId = item.StockMovementId,
                    MovementType = item.MovementType,
                    Quantity = item.Quantity,
                    Notes = item.Notes,
                    CreatedAt = item.CreatedAt,

                    ProductionLotId = item.ProductionLotId,
                    LotNumber = item.ProductionLot.LotNumber,
                    ProductionDate = item.ProductionLot.ProductionDate,
                    ExpirationDate = item.ProductionLot.ExpirationDate,

                    ProductId = item.ProductionLot.ProductionOrder.ProductId,
                    ProductCode = item.ProductionLot.ProductionOrder.Product.ProductCode,
                    ProductName = item.ProductionLot.ProductionOrder.Product.Name,

                    LogisticUnitId = item.LogisticUnitId,
                    Sscc = item.LogisticUnit != null
                        ? item.LogisticUnit.Sscc
                        : null,
                    UnitType = item.LogisticUnit != null
                        ? item.LogisticUnit.UnitType
                        : null,

                    WarehouseOrderId = item.WarehouseOrderId,
                    WarehouseOrderNumber = item.WarehouseOrder != null
                        ? item.WarehouseOrder.OrderNumber
                        : null,

                    CreatedByUserId = item.CreatedByUserId,
                    CreatedByUserName = item.CreatedByUser.FullName,
                }).ToListAsync();
        }
    }
}