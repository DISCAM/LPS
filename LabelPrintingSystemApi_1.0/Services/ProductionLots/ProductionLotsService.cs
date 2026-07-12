using Data.Dtos.ProductionLots;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.ProductionLots
{
    public class ProductionLotsService : IProductionLotsService
    {
        private readonly DatabaseContext databaseContext;
        private readonly IAuditLogService auditLogService;

        public ProductionLotsService(DatabaseContext databaseContext, 
            IAuditLogService auditLogService)
        {
            this.databaseContext = databaseContext;
            this.auditLogService = auditLogService;
        }

        public async Task<List<ProductionLotDto>> GetAllByProductionOrderIdAsync(
            int productionOrderId)
        {
            var productionLots = await this.databaseContext.ProductionLots
                .AsNoTracking()
                .Where(x => x.ProductionOrderId == productionOrderId)
                .Include(x => x.ProductionOrder)
                    .ThenInclude(x => x.Product)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ProductionLotDto
                {
                    ProductionLotId = x.ProductionLotId,

                    ProductionOrderId = x.ProductionOrderId,
                    ProductionOrderNumber = x.ProductionOrder.OrderNumber,

                    ProductId = x.ProductionOrder.ProductId,
                    ProductCode = x.ProductionOrder.Product.ProductCode,
                    ProductName = x.ProductionOrder.Product.Name,

                    LotNumber = x.LotNumber,
                    ProductionDate = x.ProductionDate,
                    ExpirationDate = x.ExpirationDate,
                    ProductionLine = x.ProductionLine,
                    ShiftCode = x.ShiftCode,

                    ProducedQuantity = x.ProducedQuantity,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return productionLots;
        }

        public async Task<ProductionLotDto> CreateAsync(
            int productionOrderId,
            CreateProductionLotDto dto,
            string identityUserId)
        {
            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                throw new BadRequestException(
                    "Nie udało się odczytać zalogowanego użytkownika.");
            }

            var user = await this.databaseContext.Users
                .FirstOrDefaultAsync(x =>
                    x.IdentityUserId == identityUserId &&
                    x.IsActive);

            if (user is null)
            {
                throw new NotFoundException(
                    "Nie znaleziono aktywnego użytkownika systemu.");
            }

            var productionOrder = await this.databaseContext.ProductionOrders
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.ProductionOrderId == productionOrderId);

            if (productionOrder is null)
            {
                throw new NotFoundException(
                    "Nie znaleziono zlecenia produkcyjnego.");
            }

            if (string.IsNullOrWhiteSpace(dto.LotNumber))
            {
                throw new BadRequestException(
                    "Numer LOT jest wymagany.");
            }

            var normalizedLotNumber = dto.LotNumber.Trim();

            var lotAlreadyExists = await this.databaseContext.ProductionLots
                .AnyAsync(x => x.LotNumber == normalizedLotNumber);

            if (lotAlreadyExists)
            {
                throw new BadRequestException(
                    "Partia o podanym numerze LOT już istnieje.");
            }

            if (dto.ExpirationDate.HasValue &&
                dto.ExpirationDate.Value < dto.ProductionDate)
            {
                throw new BadRequestException(
                    "Data ważności nie może być wcześniejsza niż data produkcji.");
            }

            var productionLot = new ProductionLot
            {
                LotNumber = normalizedLotNumber,

                ProductionDate = dto.ProductionDate,
                ExpirationDate = dto.ExpirationDate,

                ProductionLine = string.IsNullOrWhiteSpace(dto.ProductionLine)
                    ? null
                    : dto.ProductionLine.Trim(),

                ShiftCode = string.IsNullOrWhiteSpace(dto.ShiftCode)
                    ? null
                    : dto.ShiftCode.Trim().ToUpperInvariant(),

                ProductionOrderId = productionOrder.ProductionOrderId,

                ProducedQuantity = dto.ProducedQuantity,
                Status = "CREATED",

                CreatedByUserId = user.UserId,
                CreatedAt = DateTime.UtcNow
            };
            using var databaseTransaction = await databaseContext.Database.BeginTransactionAsync();
            try
            {
                this.databaseContext.ProductionLots.Add(productionLot);

                await this.databaseContext.SaveChangesAsync();
                await auditLogService.AddAsync(user.UserId, "ProductionLot", productionLot.ProductionLotId,
                    "CREATE_PRODUCTION_LOT", $"Utworzono partię produkcyjną {productionLot.LotNumber}. " +
                    $"Zlecenie produkcyjne: {productionOrder.OrderNumber}. " + $"Produkt: {productionOrder.Product.ProductCode} - " +
                    $"{productionOrder.Product.Name}. " + $"Ilość wyprodukowana: {productionLot.ProducedQuantity}. " +
                    $"Data produkcji: {productionLot.ProductionDate:yyyy-MM-dd}. " + $"Data ważności: " +
                    $"{(productionLot.ExpirationDate.HasValue ? productionLot.ExpirationDate.Value.ToString("yyyy-MM-dd")
                    : "brak")}.");
                await databaseContext.SaveChangesAsync();

                await databaseTransaction.CommitAsync();
            }
            catch {
                await databaseTransaction.RollbackAsync();

                throw;

            }

            return new ProductionLotDto
            {
                ProductionLotId = productionLot.ProductionLotId,

                ProductionOrderId = productionOrder.ProductionOrderId,
                ProductionOrderNumber = productionOrder.OrderNumber,

                ProductId = productionOrder.ProductId,
                ProductCode = productionOrder.Product.ProductCode,
                ProductName = productionOrder.Product.Name,

                LotNumber = productionLot.LotNumber,
                ProductionDate = productionLot.ProductionDate,
                ExpirationDate = productionLot.ExpirationDate,
                ProductionLine = productionLot.ProductionLine,
                ShiftCode = productionLot.ShiftCode,

                ProducedQuantity = productionLot.ProducedQuantity,
                Status = productionLot.Status,
                CreatedAt = productionLot.CreatedAt
            };
        }
    }
}
