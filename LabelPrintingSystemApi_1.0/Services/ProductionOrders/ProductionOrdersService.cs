using Data.Dtos.ProductionOrders;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.ProductionOrders
{
    public class ProductionOrdersService : IProductionOrdersService
    {
        private readonly DatabaseContext databaseContext;
        private readonly IAuditLogService auditLogService;

        public ProductionOrdersService(DatabaseContext databaseContext, IAuditLogService auditLogService)
        {
            this.databaseContext = databaseContext;
            this.auditLogService = auditLogService;
        }

        public async Task<List<ProductionOrderDto>> GetAllAsync()
        {
            var productionOrders = await this.databaseContext.ProductionOrders
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Customer)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ProductionOrderDto
                {
                    ProductionOrderId = x.ProductionOrderId,
                    OrderNumber = x.OrderNumber,
                    ProductId = x.ProductId,
                    ProductCode = x.Product.ProductCode,
                    ProductName = x.Product.Name,
                    PlannedQuantity = x.PlannedQuantity,
                    Status = x.Status,
                    PlannedStartDate = x.PlannedStartDate,
                    PlannedEndDate = x.PlannedEndDate,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer != null
                        ? x.Customer.Name
                        : null,

                    ProductionOrderType = x.ProductionOrderType,
                    CreatedAt = x.CreatedAt
                }).ToListAsync();
            return productionOrders;
        }

        public async Task<ProductionOrderDto> CreateAsync(
            CreateProductionOrderDto dto,
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

            if (string.IsNullOrWhiteSpace(dto.OrderNumber))
            {
                throw new BadRequestException(
                    "Numer zlecenia jest wymagany.");
            }

            var normalizedOrderNumber = dto.OrderNumber.Trim();

            var orderAlreadyExists = await databaseContext.ProductionOrders
                .AnyAsync(x => x.OrderNumber == normalizedOrderNumber);

            if (orderAlreadyExists)
            {
                throw new BadRequestException(
                    "Zlecenie o podanym numerze już istnieje.");
            }

            if (dto.PlannedStartDate.HasValue &&
                dto.PlannedEndDate.HasValue &&
                dto.PlannedEndDate.Value < dto.PlannedStartDate.Value)
            {
                throw new BadRequestException(
                    "Planowana data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
            }

            var productionOrderType =
                string.IsNullOrWhiteSpace(dto.ProductionOrderType)
                    ? "STOCK"
                    : dto.ProductionOrderType.Trim().ToUpperInvariant();

            if (productionOrderType != "STOCK" &&
                productionOrderType != "CUSTOMER_ORDER")
            {
                throw new BadRequestException(
                    "Nieprawidłowy typ zlecenia produkcyjnego.");
            }

            if (productionOrderType == "STOCK" &&
                dto.CustomerId.HasValue)
            {
                throw new BadRequestException(
                    "Zlecenie typu STOCK nie może mieć przypisanego klienta.");
            }

            if (productionOrderType == "CUSTOMER_ORDER" &&
                !dto.CustomerId.HasValue)
            {
                throw new BadRequestException(
                    "Zlecenie typu CUSTOMER_ORDER wymaga wskazania klienta.");
            }

            var product = await this.databaseContext.Products
                .FirstOrDefaultAsync(x =>
                    x.ProductId == dto.ProductId &&
                    x.IsActive);

            if (product is null)
            {
                throw new NotFoundException(
                    "Nie znaleziono aktywnego produktu.");
            }

            Customer? customer = null;

            if (dto.CustomerId.HasValue)
            {
                customer = await this.databaseContext.Customers
                    .FirstOrDefaultAsync(x =>
                        x.CustomerId == dto.CustomerId.Value &&
                        x.IsActive);

                if (customer is null)
                {
                    throw new NotFoundException(
                        "Nie znaleziono aktywnego klienta.");
                }
            }

            var productionOrder = new ProductionOrder
            {
                OrderNumber = normalizedOrderNumber,
                ProductId = product.ProductId,
                PlannedQuantity = dto.PlannedQuantity,

                Status = "NEW",

                PlannedStartDate = dto.PlannedStartDate,
                PlannedEndDate = dto.PlannedEndDate,

                CustomerId = dto.CustomerId,
                ProductionOrderType = productionOrderType,

                CreatedByUserId = user.UserId,
                CreatedAt = DateTime.UtcNow
            };

            using var databaseTransaction = await this.databaseContext.Database.BeginTransactionAsync();

            try{

                databaseContext.ProductionOrders.Add(productionOrder);

                await databaseContext.SaveChangesAsync();

                await auditLogService.AddAsync(user.UserId, "ProductionOrder", productionOrder.ProductionOrderId, "CREATE_PRODUCTION_ORDER",
                    $"Utworzono zlecenie produkcyjne {productionOrder.OrderNumber}. " + $"Produkt: {product.ProductCode} - {product.Name}. " +
                    $"Planowana ilość: {productionOrder.PlannedQuantity}. " + $"Typ zlecenia: {productionOrder.ProductionOrderType}. " +
                    $"Klient: {customer?.Name ?? "produkcja na magazyn"}.");

                await this.databaseContext.SaveChangesAsync();

                await databaseTransaction.CommitAsync();
            }
            catch {

                await databaseTransaction.RollbackAsync();

                throw;
            }

            return new ProductionOrderDto
            {
                ProductionOrderId = productionOrder.ProductionOrderId,
                OrderNumber = productionOrder.OrderNumber,

                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.Name,

                PlannedQuantity = productionOrder.PlannedQuantity,
                Status = productionOrder.Status,

                PlannedStartDate = productionOrder.PlannedStartDate,
                PlannedEndDate = productionOrder.PlannedEndDate,

                CustomerId = customer?.CustomerId,
                CustomerName = customer?.Name,

                ProductionOrderType = productionOrder.ProductionOrderType,
                CreatedAt = productionOrder.CreatedAt
            };
        }
    }
}