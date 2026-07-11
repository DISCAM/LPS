using Data.Dtos.WarehouseOrders;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.WarehouseOrders
{
    public class WarehouseOrdersService : IWarehouseOrdersService
    {
        private const string ORDER_TYPE_SHIPMENT = "SHIPMENT";
        private const string WAREHOUSE_ORDER_STATUS_NEW = "NEW";
        private const string WAREHOUSE_ORDER_STATUS_COMPLETED = "COMPLETED";
        private const string LOGISTIC_UNIT_STATUS_SHIPPED = "SHIPPED";
        private const string STOCK_MOVEMENT_TYPE_SHIPMENT = "SHIPMENT";
        private readonly DatabaseContext databaseContext;

        public WarehouseOrdersService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<WarehouseOrderDto>> GetAllAsync()
        {
            return await this.databaseContext.WarehouseOrders
                .AsNoTracking()
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new WarehouseOrderDto
                {
                    WarehouseOrderId = item.WarehouseOrderId,
                    OrderNumber = item.OrderNumber,
                    OrderType = item.OrderType,
                    Status = item.Status,
                    DeliveryAddress = item.DeliveryAddress,
                    CustomerId = item.CustomerId,
                    CustomerName = item.Customer != null
                        ? item.Customer.Name
                        : null,
                    CreatedByUserId = item.CreatedByUserId,
                    CreatedByUserName = item.CreatedByUser.FullName,
                    CreatedAt = item.CreatedAt,
                    LogisticUnitsCount = item.LogisticUnits.Count,
                    TotalQuantity = item.LogisticUnits
                        .SelectMany(logisticUnit => logisticUnit.LogisticUnitItems)
                        .Sum(logisticUnitItem => (decimal?)logisticUnitItem.Quantity) ?? 0,
                }).ToListAsync();
        }

        public async Task<WarehouseOrderDetailsDto> GetByIdAsync(int id)
        {
            WarehouseOrderDetailsDto? result =
                await this.databaseContext.WarehouseOrders
                    .AsNoTracking()
                    .Where(item => item.WarehouseOrderId == id)
                    .Select(item => new WarehouseOrderDetailsDto
                    {
                        WarehouseOrderId = item.WarehouseOrderId,
                        OrderNumber = item.OrderNumber,
                        OrderType = item.OrderType,
                        Status = item.Status,
                        DeliveryAddress = item.DeliveryAddress,
                        CustomerId = item.CustomerId,
                        CustomerName = item.Customer != null
                            ? item.Customer.Name
                            : null,
                        CreatedByUserId = item.CreatedByUserId,
                        CreatedByUserName = item.CreatedByUser.FullName,
                        CreatedAt = item.CreatedAt,
                        ModifiedByUserId = item.ModifiedByUserId,
                        ModifiedAt = item.ModifiedAt,

                        LogisticUnits = item.LogisticUnits
                            .OrderByDescending(logisticUnit =>
                                logisticUnit.CreatedAt
                            )
                            .Select(logisticUnit =>
                                new WarehouseOrderLogisticUnitDto
                                {
                                    LogisticUnitId = logisticUnit.LogisticUnitId,

                                    Sscc = logisticUnit.Sscc,

                                    UnitType = logisticUnit.UnitType,

                                    Status = logisticUnit.Status,

                                    CreatedAt = logisticUnit.CreatedAt,

                                    TotalQuantity = logisticUnit
                                        .LogisticUnitItems
                                        .Sum(logisticUnitItem => logisticUnitItem.Quantity),

                                    Items = logisticUnit.LogisticUnitItems
                                        .OrderBy(logisticUnitItem =>
                                            logisticUnitItem
                                                .LogisticUnitItemId
                                        )
                                        .Select(logisticUnitItem =>
                                            new WarehouseOrderLogisticUnitItemDto
                                            {
                                                LogisticUnitItemId =
                                                    logisticUnitItem
                                                        .LogisticUnitItemId,

                                                ProductionLotId =
                                                    logisticUnitItem
                                                        .ProductionLotId,

                                                LotNumber =
                                                    logisticUnitItem
                                                        .ProductionLot
                                                        .LotNumber,

                                                ProductId =
                                                    logisticUnitItem
                                                        .ProductionLot
                                                        .ProductionOrder
                                                        .ProductId,

                                                ProductCode =
                                                    logisticUnitItem
                                                        .ProductionLot
                                                        .ProductionOrder
                                                        .Product
                                                        .ProductCode,

                                                ProductName =
                                                    logisticUnitItem
                                                        .ProductionLot
                                                        .ProductionOrder
                                                        .Product
                                                        .Name,

                                                Quantity =
                                                    logisticUnitItem
                                                        .Quantity,
                                            }
                                        ).ToList(),
                                }
                            ).ToList(),
                    }).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new NotFoundException(
                    $"Nie znaleziono zlecenia magazynowego o ID {id}."
                );
            }

            return result;
        }

        public async Task<WarehouseOrderDetailsDto> CreateAsync(
            CreateWarehouseOrderDto dto,
            string identityUserId
        )
        {
            User user = await this.GetActiveUserAsync(identityUserId);

            string orderNumber = dto.OrderNumber.Trim();

            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new BadRequestException(
                    "Numer zlecenia magazynowego jest wymagany."
                );
            }

            bool orderNumberExists =
                await this.databaseContext.WarehouseOrders
                    .AnyAsync(item => item.OrderNumber == orderNumber);

            if (orderNumberExists)
            {
                throw new BadRequestException(
                    "Zlecenie magazynowe o podanym numerze już istnieje."
                );
            }

            string orderType = string.IsNullOrWhiteSpace(dto.OrderType)
                ? ORDER_TYPE_SHIPMENT
                : dto.OrderType.Trim().ToUpperInvariant();

            if (orderType != ORDER_TYPE_SHIPMENT)
            {
                throw new BadRequestException(
                    "Na tym etapie obsługiwany jest tylko typ zlecenia SHIPMENT."
                );
            }

            if (dto.CustomerId.HasValue)
            {
                bool customerExists =
                    await this.databaseContext.Customers
                        .AnyAsync(item =>
                            item.CustomerId == dto.CustomerId.Value &&
                            item.IsActive
                        );

                if (!customerExists)
                {
                    throw new BadRequestException(
                        "Wybrany klient nie istnieje albo jest nieaktywny."
                    );
                }
            }

            DateTime now = DateTime.Now;

            WarehouseOrder warehouseOrder = new()
            {
                OrderNumber = orderNumber,
                OrderType = orderType,
                DeliveryAddress = string.IsNullOrWhiteSpace(dto.DeliveryAddress)
                    ? null
                    : dto.DeliveryAddress.Trim(),
                Status = WAREHOUSE_ORDER_STATUS_NEW,
                CustomerId = dto.CustomerId,
                CreatedByUserId = user.UserId,
                CreatedAt = now,
            };

            await this.databaseContext.WarehouseOrders.AddAsync(
                warehouseOrder
            );

            await this.databaseContext.SaveChangesAsync();

            return await this.GetByIdAsync(warehouseOrder.WarehouseOrderId);
        }

        public async Task<ShipLogisticUnitResultDto> ShipLogisticUnitAsync(
            int warehouseOrderId,
            ShipLogisticUnitDto dto,
            string identityUserId
        )
        {
            User user = await this.GetActiveUserAsync(identityUserId);

            using var databaseTransaction =
                await this.databaseContext.Database.BeginTransactionAsync();

            try
            {
                WarehouseOrder warehouseOrder =
                    await this.databaseContext.WarehouseOrders
                        .FirstOrDefaultAsync(item =>
                            item.WarehouseOrderId == warehouseOrderId
                        )
                    ?? throw new NotFoundException(
                        $"Nie znaleziono zlecenia magazynowego o ID {warehouseOrderId}."
                    );

                if (warehouseOrder.Status == WAREHOUSE_ORDER_STATUS_COMPLETED)
                {
                    throw new BadRequestException(
                        "Zlecenie magazynowe jest już zakończone."
                    );
                }

                LogisticUnit logisticUnit =
                    await this.databaseContext.LogisticUnits
                        .Include(item => item.LogisticUnitItems)
                        .FirstOrDefaultAsync(item =>
                            item.LogisticUnitId == dto.LogisticUnitId
                        )
                    ?? throw new NotFoundException(
                        $"Nie znaleziono jednostki logistycznej o ID {dto.LogisticUnitId}."
                    );

                if (logisticUnit.Status == LOGISTIC_UNIT_STATUS_SHIPPED)
                {
                    throw new BadRequestException(
                        "Jednostka logistyczna została już wydana z magazynu."
                    );
                }

                if (logisticUnit.WarehouseOrderId.HasValue)
                {
                    throw new BadRequestException(
                        "Jednostka logistyczna jest już przypisana do zlecenia magazynowego."
                    );
                }

                if (!logisticUnit.LogisticUnitItems.Any())
                {
                    throw new BadRequestException(
                        "Jednostka logistyczna nie ma żadnej zawartości."
                    );
                }

                DateTime now = DateTime.Now;

                logisticUnit.WarehouseOrderId = warehouseOrder.WarehouseOrderId;
                logisticUnit.Status = LOGISTIC_UNIT_STATUS_SHIPPED;
                logisticUnit.ModifiedByUserId = user.UserId;
                logisticUnit.ModifiedAt = now;

                warehouseOrder.Status = WAREHOUSE_ORDER_STATUS_COMPLETED;
                warehouseOrder.ModifiedByUserId = user.UserId;
                warehouseOrder.ModifiedAt = now;

                string? notes = string.IsNullOrWhiteSpace(dto.Notes)
                    ? null
                    : dto.Notes.Trim();

                List<StockMovement> stockMovements =
                    logisticUnit.LogisticUnitItems
                        .Select(logisticUnitItem => new StockMovement
                        {
                            ProductionLotId =
                                logisticUnitItem.ProductionLotId,

                            WarehouseOrderId =
                                warehouseOrder.WarehouseOrderId,

                            LogisticUnitId =
                                logisticUnit.LogisticUnitId,

                            MovementType =
                                STOCK_MOVEMENT_TYPE_SHIPMENT,

                            Quantity =
                                logisticUnitItem.Quantity,

                            Notes =
                                notes,

                            CreatedByUserId =
                                user.UserId,

                            CreatedAt =
                                now,
                        })
                        .ToList();

                await this.databaseContext.StockMovements.AddRangeAsync(
                    stockMovements
                );

                await this.databaseContext.SaveChangesAsync();

                await databaseTransaction.CommitAsync();

                decimal totalQuantity =
                    logisticUnit.LogisticUnitItems.Sum(item => item.Quantity);

                return new ShipLogisticUnitResultDto
                {
                    WarehouseOrderId = warehouseOrder.WarehouseOrderId,
                    OrderNumber = warehouseOrder.OrderNumber,
                    WarehouseOrderStatus = warehouseOrder.Status,
                    LogisticUnitId = logisticUnit.LogisticUnitId,
                    Sscc = logisticUnit.Sscc,
                    LogisticUnitStatus = logisticUnit.Status,
                    Quantity = totalQuantity,
                    MovementType = STOCK_MOVEMENT_TYPE_SHIPMENT,
                    StockMovementIds = stockMovements
                        .Select(item => item.StockMovementId)
                        .ToList(),
                };
            }
            catch
            {
                await databaseTransaction.RollbackAsync();

                throw;
            }
        }

        private async Task<User> GetActiveUserAsync(string identityUserId)
        {
            return await this.databaseContext.Users
                .FirstOrDefaultAsync(item =>
                    item.IdentityUserId == identityUserId &&
                    item.IsActive
                )
                ?? throw new BadRequestException(
                    "Nie znaleziono aktywnego użytkownika wykonującego operację."
                );
        }
    }
}