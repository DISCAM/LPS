using System.Text.Json;
using Data.Dtos.PrintLabel;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.PrintLabel
{
    public class PrintLabelService : IPrintLabelService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<PrintLabelService> logger;

        private static readonly JsonSerializerOptions labelDataJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public PrintLabelService(
            DatabaseContext databaseContext,
            ILogger<PrintLabelService> logger
        )
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
        }

        public async Task<PrintResultDto> PrintEanAsync(PrintEanDto dto, string identityUserId)
        {
            User currentUser = await databaseContext.Users
                .FirstOrDefaultAsync(item =>
                    item.IdentityUserId == identityUserId &&
                    item.IsActive)
                ?? throw new NotFoundException("Active user not found");

            Product product = await databaseContext.Products
                .FirstOrDefaultAsync(item =>
                    item.ProductId == dto.ProductId &&
                    item.IsActive)
                ?? throw new NotFoundException("Product not found");

            if (string.IsNullOrWhiteSpace(product.Ean))
            {
                throw new BadRequestException(
                    "Product does not have an EAN code"
                );
            }

            LabelTemplate labelTemplate = await databaseContext.LabelTemplates
                .FirstOrDefaultAsync(item =>
                    item.LabelTemplateId == dto.LabelTemplateId &&
                    item.IsActive &&
                    item.LabelType == "PRODUCT")
                ?? throw new NotFoundException(
                    "Active PRODUCT label template not found"
                );

            Printer printer = await databaseContext.Printers
                .FirstOrDefaultAsync(item =>
                    item.PrinterId == dto.PrinterId &&
                    item.IsActive)
                ?? throw new NotFoundException("Active printer not found");

            PrintEanLabelDataDto labelData = new PrintEanLabelDataDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.Name,
                Description = product.Description,
                Ean = product.Ean,
                Gtin = product.Gtin,
                LabelTemplateId = labelTemplate.LabelTemplateId,
                TemplateName = labelTemplate.Name,
                TemplateReference = labelTemplate.TemplateReference,
                TemplateVersionNo = labelTemplate.VersionNo,
            };

            string labelDataJson = JsonSerializer.Serialize(
                labelData,
                labelDataJsonOptions
            );

            DateTime createdAt = DateTime.UtcNow;

            await using var transaction =
                await databaseContext.Database.BeginTransactionAsync();

            try
            {
                Label label = new Label
                {
                    LabelType = "PRODUCT",
                    ProductId = product.ProductId,
                    LabelTemplateId = labelTemplate.LabelTemplateId,
                    PrimaryCodeValue = product.Ean,
                    LabelDataJson = labelDataJson,
                    CreatedByUserId = currentUser.UserId,
                    CreatedAt = createdAt
                };

                databaseContext.Labels.Add(label);

                await databaseContext.SaveChangesAsync();

                PrintJob printJob = new PrintJob
                {
                    LabelId = label.LabelId,
                    PrinterId = printer.PrinterId,
                    CreatedByUserId = currentUser.UserId,
                    Copies = dto.Copies,
                    Status = "QUEUED",
                    IsReprint = false,
                    CreatedAt = createdAt
                };

                databaseContext.PrintJobs.Add(printJob);

                await databaseContext.SaveChangesAsync();

                PrintJobHistory printJobHistory = new PrintJobHistory
                {
                    PrintJobId = printJob.PrintJobId,
                    Status = "QUEUED",
                    CreatedAt = createdAt,
                    Note = "Utworzono zadanie wydruku."
                };

                databaseContext.PrintJobHistories.Add(printJobHistory);

                await databaseContext.SaveChangesAsync();

                await transaction.CommitAsync();

                logger.LogInformation(
                    "EAN print job was created. LabelId: {LabelId}, PrintJobId: {PrintJobId}, ProductId: {ProductId}, UserId: {UserId}",
                    label.LabelId,
                    printJob.PrintJobId,
                    product.ProductId,
                    currentUser.UserId
                );

                return new PrintResultDto
                {
                    LabelId = label.LabelId,
                    PrintJobId = printJob.PrintJobId,
                    LabelType = label.LabelType,
                    PrimaryCodeValue = label.PrimaryCodeValue ?? string.Empty,
                    LabelTemplateId = labelTemplate.LabelTemplateId,
                    TemplateName = labelTemplate.Name,
                    PrinterId = printer.PrinterId,
                    PrinterName = printer.Name,
                    CreatedByUserId = currentUser.UserId,
                    Copies = printJob.Copies,
                    Status = printJob.Status,
                    IsReprint = printJob.IsReprint,
                    CreatedAt = printJob.CreatedAt
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PrintResultDto> PrintProductionLabelAsync(PrintProductionLabelDto dto, string identityUserId)
        {
            User currentUser = await databaseContext.Users
                .FirstOrDefaultAsync(item =>
                    item.IdentityUserId == identityUserId &&
                    item.IsActive)
                ?? throw new NotFoundException("Active user not found");

            ProductionLot productionLot = await databaseContext.ProductionLots
                .Include(item => item.ProductionOrder)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item =>
                    item.ProductionLotId == dto.ProductionLotId)
                ?? throw new NotFoundException(
                    "Production lot not found"
                );

            Product product = productionLot.ProductionOrder.Product;

            if (!product.IsActive)
            {
                throw new BadRequestException(
                    "Product assigned to production lot is inactive"
                );
            }

            if (string.IsNullOrWhiteSpace(product.Ean))
            {
                throw new BadRequestException(
                    "Product does not have an EAN code"
                );
            }

            LabelTemplate labelTemplate = await databaseContext.LabelTemplates
                .FirstOrDefaultAsync(item =>
                    item.LabelTemplateId == dto.LabelTemplateId &&
                    item.IsActive &&
                    item.LabelType == "PRODUCTION")
                ?? throw new NotFoundException(
                    "Active PRODUCTION label template not found"
                );

            Printer printer = await databaseContext.Printers
                .FirstOrDefaultAsync(item =>
                    item.PrinterId == dto.PrinterId &&
                    item.IsActive)
                ?? throw new NotFoundException(
                    "Active printer not found"
                );

            PrintProductionLabelDataDto labelData = new PrintProductionLabelDataDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.Name,
                Description = product.Description,
                Ean = product.Ean,
                Gtin = product.Gtin,

                LabelTemplateId = labelTemplate.LabelTemplateId,
                TemplateName = labelTemplate.Name,
                TemplateReference = labelTemplate.TemplateReference,
                TemplateVersionNo = labelTemplate.VersionNo,

                ProductionLotId = productionLot.ProductionLotId,
                ProductionOrderId = productionLot.ProductionOrderId,
                ProductionOrderNumber =
                        productionLot.ProductionOrder.OrderNumber,

                LotNumber = productionLot.LotNumber,
                ProductionDate = productionLot.ProductionDate,
                ExpirationDate = productionLot.ExpirationDate,
                ProductionLine = productionLot.ProductionLine,
                ShiftCode = productionLot.ShiftCode,
                ProducedQuantity = productionLot.ProducedQuantity
            };

            string labelDataJson = JsonSerializer.Serialize(
                labelData,
                labelDataJsonOptions
            );

            DateTime createdAt = DateTime.UtcNow;

            await using var transaction =
                await databaseContext.Database.BeginTransactionAsync();

            try
            {
                Label label = new Label
                {
                    LabelType = "PRODUCTION",

                    ProductId = product.ProductId,
                    ProductionLotId = productionLot.ProductionLotId,

                    LabelTemplateId = labelTemplate.LabelTemplateId,

                    PrimaryCodeValue = productionLot.LotNumber,
                    LabelDataJson = labelDataJson,

                    CreatedByUserId = currentUser.UserId,
                    CreatedAt = createdAt
                };

                databaseContext.Labels.Add(label);

                await databaseContext.SaveChangesAsync();

                PrintJob printJob = new PrintJob
                {
                    LabelId = label.LabelId,
                    PrinterId = printer.PrinterId,
                    CreatedByUserId = currentUser.UserId,
                    Copies = dto.Copies,
                    Status = "QUEUED",
                    IsReprint = false,
                    CreatedAt = createdAt
                };

                databaseContext.PrintJobs.Add(printJob);

                await databaseContext.SaveChangesAsync();

                PrintJobHistory printJobHistory = new PrintJobHistory
                {
                    PrintJobId = printJob.PrintJobId,
                    Status = "QUEUED",
                    CreatedAt = createdAt,
                    Note = "Utworzono zadanie wydruku etykiety produkcyjnej."
                };

                databaseContext.PrintJobHistories.Add(printJobHistory);

                await databaseContext.SaveChangesAsync();

                await transaction.CommitAsync();

                logger.LogInformation(
                    "Production label print job was created. LabelId: {LabelId}, PrintJobId: {PrintJobId}, ProductionLotId: {ProductionLotId}, UserId: {UserId}",
                    label.LabelId,
                    printJob.PrintJobId,
                    productionLot.ProductionLotId,
                    currentUser.UserId
                );

                return new PrintResultDto
                {
                    LabelId = label.LabelId,
                    PrintJobId = printJob.PrintJobId,
                    LabelType = label.LabelType,
                    PrimaryCodeValue = label.PrimaryCodeValue ?? string.Empty,
                    LabelTemplateId = labelTemplate.LabelTemplateId,
                    TemplateName = labelTemplate.Name,
                    PrinterId = printer.PrinterId,
                    PrinterName = printer.Name,
                    CreatedByUserId = currentUser.UserId,
                    Copies = printJob.Copies,
                    Status = printJob.Status,
                    IsReprint = printJob.IsReprint,
                    CreatedAt = printJob.CreatedAt
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}