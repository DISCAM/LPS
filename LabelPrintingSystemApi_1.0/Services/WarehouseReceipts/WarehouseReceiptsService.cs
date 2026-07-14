using System.Text.Json;
using Data.Dtos.PrintLabel;
using Data.Dtos.WarehouseReceipts;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.WarehouseReceipts
{
    public class WarehouseReceiptsService : IWarehouseReceiptsService
    {
        private const string PRODUCTION_RECEIPT = "PRODUCTION_RECEIPT";


        private static readonly JsonSerializerOptions labelDataJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly DatabaseContext databaseContext;
        private readonly IAuditLogService auditLogService;

        public WarehouseReceiptsService(DatabaseContext databaseContext, IAuditLogService auditLogService)
        {
            this.databaseContext = databaseContext;
            this.auditLogService = auditLogService;

        }

        public async Task<WarehouseReceiptResultDto> CreateAsync(
            CreateWarehouseReceiptDto dto,
            string identityUserId
        )
        {
            User user = await this.databaseContext.Users
                .FirstOrDefaultAsync(item =>
                    item.IdentityUserId == identityUserId &&
                    item.IsActive
                )
                ?? throw new NotFoundException(
                    "Nie znaleziono aktywnego użytkownika."
                );

            ProductionLot productionLot = await this.databaseContext.ProductionLots
                .Include(item => item.ProductionOrder)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item =>
                    item.ProductionLotId == dto.ProductionLotId
                )
                ?? throw new NotFoundException(
                    $"Nie znaleziono partii produkcyjnej o ID {dto.ProductionLotId}."
                );

            Product product = productionLot.ProductionOrder.Product;

            if (!product.IsActive)
            {
                throw new BadRequestException(
                    "Produkt przypisany do partii produkcyjnej jest nieaktywny."
                );
            }

            LabelTemplate labelTemplate = await this.databaseContext.LabelTemplates
                .FirstOrDefaultAsync(item =>
                    item.LabelTemplateId == dto.LabelTemplateId &&
                    item.IsActive
                )
                ?? throw new NotFoundException(
                    $"Nie znaleziono aktywnego szablonu etykiety o ID {dto.LabelTemplateId}."
                );

            if (labelTemplate.LabelType != "LOGISTIC")
            {
                throw new BadRequestException(
                    "Wybrany szablon nie jest szablonem etykiety logistycznej."
                );
            }

            Printer printer = await this.databaseContext.Printers
                .FirstOrDefaultAsync(item =>
                    item.PrinterId == dto.PrinterId &&
                    item.IsActive
                )
                ?? throw new NotFoundException(
                    $"Nie znaleziono aktywnej drukarki o ID {dto.PrinterId}."
                );

            string unitType = dto.UnitType.Trim().ToUpperInvariant();

            if (!IsAllowedUnitType(unitType))
            {
                throw new BadRequestException(
                    "Nieprawidłowy typ jednostki logistycznej. " +
                    "Dozwolone wartości: BOX, CARTON, PALLET, OTHER."
                );
            }

            decimal alreadyReceivedQuantity = await this.databaseContext.StockMovements
                .Where(item =>
                    item.ProductionLotId == productionLot.ProductionLotId &&
                    item.MovementType == PRODUCTION_RECEIPT
                ).SumAsync(item => (decimal?)item.Quantity) ?? 0m;

            decimal availableQuantity =
                productionLot.ProducedQuantity - alreadyReceivedQuantity;

            if (dto.Quantity > availableQuantity)
            {
                throw new BadRequestException(
                    $"Nie można przyjąć {dto.Quantity}. " +
                    $"Dostępna ilość do przyjęcia: {availableQuantity}."
                );
            }

            using var databaseTransaction =
                await this.databaseContext.Database.BeginTransactionAsync();

            try
            {
                DateTime now = DateTime.Now;

                string sscc = await this.GenerateUniqueSsccAsync();

                LogisticUnit logisticUnit = new()
                {
                    Sscc = sscc,
                    UnitType = unitType,
                    Status = "CREATED",
                    WarehouseOrderId = null,
                    CreatedByUserId = user.UserId,
                    CreatedAt = now,
                };

                await this.databaseContext.LogisticUnits.AddAsync(logisticUnit);



                await this.databaseContext.SaveChangesAsync();

                LogisticUnitItem logisticUnitItem = new()
                {
                    LogisticUnitId = logisticUnit.LogisticUnitId,
                    WarehouseOrderItemId = null,
                    ProductionLotId = productionLot.ProductionLotId,
                    Quantity = dto.Quantity,
                    CreatedByUserId = user.UserId,
                    CreatedAt = now,
                };

                await this.databaseContext.LogisticUnitItems.AddAsync(
                    logisticUnitItem
                );

                StockMovement stockMovement = new()
                {
                    ProductionLotId = productionLot.ProductionLotId,
                    WarehouseOrderId = null,
                    LogisticUnitId = logisticUnit.LogisticUnitId,
                    MovementType = PRODUCTION_RECEIPT,
                    Quantity = dto.Quantity,
                    Notes = dto.Notes,
                    CreatedByUserId = user.UserId,
                    CreatedAt = now,
                };

                await this.databaseContext.StockMovements.AddAsync(stockMovement);

                await this.databaseContext.SaveChangesAsync();

                PrintLogisticLabelDataDto labelData = new()
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.Name,
                    Description = product.Description,
                    Ean = product.Ean,
                    Gtin = product.Gtin,

                    TemplateReference = labelTemplate.TemplateReference,
                    TemplateVersionNo = labelTemplate.VersionNo,

                    LogisticUnitId = logisticUnit.LogisticUnitId,
                    Sscc = logisticUnit.Sscc,
                    UnitType = logisticUnit.UnitType,

                    ProductionLotId = productionLot.ProductionLotId,
                    LotNumber = productionLot.LotNumber,
                    ProductionDate = productionLot.ProductionDate,
                    ExpirationDate = productionLot.ExpirationDate,

                    Quantity = dto.Quantity,
                };

                string labelDataJson = JsonSerializer.Serialize(
                    labelData,
                    labelDataJsonOptions
                );

                Label label = new()
                {
                    LabelType = "LOGISTIC",
                    ProductId = product.ProductId,
                    ProductionLotId = productionLot.ProductionLotId,
                    LogisticUnitId = logisticUnit.LogisticUnitId,
                    LabelTemplateId = labelTemplate.LabelTemplateId,
                    PrimaryCodeValue = logisticUnit.Sscc,
                    LabelDataJson = labelDataJson,
                    CreatedByUserId = user.UserId,
                    CreatedAt = now,
                };

                await this.databaseContext.Labels.AddAsync(label);

                await this.databaseContext.SaveChangesAsync();

                PrintJob printJob = new()
                {
                    LabelId = label.LabelId,
                    PrinterId = printer.PrinterId,
                    CreatedByUserId = user.UserId,
                    Copies = dto.Copies,
                    Status = "QUEUED",
                    IsReprint = false,
                    CreatedAt = now,
                };

                await this.databaseContext.PrintJobs.AddAsync(printJob);

                await this.databaseContext.SaveChangesAsync();

                PrintJobHistory printJobHistory = new()
                {
                    PrintJobId = printJob.PrintJobId,
                    Status = printJob.Status,
                    Note = "Utworzono zadanie wydruku etykiety logistycznej.",
                    CreatedAt = now,
                };

                await this.databaseContext.PrintJobHistories.AddAsync(printJobHistory);

                await this.auditLogService.AddAsync(user.UserId, "LogisticUnit", logisticUnit.LogisticUnitId, "CREATE_WAREHOUSE_RECEIPT",
                    $"Przyjęto LOT {productionLot.LotNumber} na magazyn. " + $"Utworzono jednostkę SSCC {logisticUnit.Sscc}. " +
                    $"Ilość: {dto.Quantity}. " + $"Typ jednostki: {logisticUnit.UnitType}. " 
                    + $"Utworzono zadanie wydruku ID {printJob.PrintJobId}.");


                await this.databaseContext.SaveChangesAsync();

                await databaseTransaction.CommitAsync();

                return new WarehouseReceiptResultDto
                {
                    LogisticUnitId = logisticUnit.LogisticUnitId,
                    Sscc = logisticUnit.Sscc,
                    LogisticUnitItemId = logisticUnitItem.LogisticUnitItemId,
                    StockMovementId = stockMovement.StockMovementId,
                    ProductionLotId = productionLot.ProductionLotId,
                    LotNumber = productionLot.LotNumber,
                    Quantity = dto.Quantity,
                    UnitType = logisticUnit.UnitType,
                    MovementType = stockMovement.MovementType,
                    Status = logisticUnit.Status,
                    LabelId = label.LabelId,
                    PrintJobId = printJob.PrintJobId,
                    PrintJobStatus = printJob.Status,
                };
            }
            catch
            {
                await databaseTransaction.RollbackAsync();

                throw;
            }
        }

        private static bool IsAllowedUnitType(string unitType)
        {
            return unitType == "BOX" ||
                   unitType == "CARTON" ||
                   unitType == "PALLET" ||
                   unitType == "OTHER";
        }

        private async Task<string> GenerateUniqueSsccAsync()
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                string sscc = GenerateSscc();

                bool exists = await this.databaseContext.LogisticUnits
                    .AnyAsync(item => item.Sscc == sscc);

                if (!exists)
                {
                    return sscc;
                }
            }

            throw new InvalidOperationException(
                "Nie udało się wygenerować unikalnego numeru SSCC."
            );
        }

        private static string GenerateSscc()
        {
            string valueWithoutCheckDigit =
                $"0{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(0, 99):D2}";

            int checkDigit = CalculateGs1CheckDigit(valueWithoutCheckDigit);

            return $"{valueWithoutCheckDigit}{checkDigit}";
        }

        private static int CalculateGs1CheckDigit(string value)
        {
            int sum = 0;
            bool multiplyByThree = true;

            for (int index = value.Length - 1; index >= 0; index--)
            {
                int digit = value[index] - '0';

                sum += digit * (multiplyByThree ? 3 : 1);

                multiplyByThree = !multiplyByThree;
            }

            return (10 - sum % 10) % 10;
        }
    }
}