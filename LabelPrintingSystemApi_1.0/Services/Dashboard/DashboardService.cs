using Data.Dtos.Dashboard;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly DatabaseContext databaseContext;

        public DashboardService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            DateTime today = DateTime.Today;

            DateTime tomorrow = today.AddDays(1);

            DashboardSummaryDto result = new()
            {
                ActiveProductionOrdersCount =
                    await this.databaseContext.ProductionOrders
                        .AsNoTracking()
                        .CountAsync(item =>
                            item.Status == "NEW" ||
                            item.Status == "IN_PROGRESS"
                        ),

                ProductionLotsTodayCount =
                    await this.databaseContext.ProductionLots
                        .AsNoTracking()
                        .CountAsync(item =>
                            item.CreatedAt >= today &&
                            item.CreatedAt < tomorrow
                        ),

                LogisticUnitsInStockCount =
                    await this.databaseContext.LogisticUnits
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "CREATED"),

                LogisticUnitsShippedCount =
                    await this.databaseContext.LogisticUnits
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "SHIPPED"),

                WarehouseOrdersNewCount =
                    await this.databaseContext.WarehouseOrders
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "NEW"),

                WarehouseOrdersCompletedCount =
                    await this.databaseContext.WarehouseOrders
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "COMPLETED"),

                PrintJobsQueuedCount =
                    await this.databaseContext.PrintJobs
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "QUEUED"),

                PrintJobsSentCount =
                    await this.databaseContext.PrintJobs
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "SENT"),

                PrintJobsErrorCount =
                    await this.databaseContext.PrintJobs
                        .AsNoTracking()
                        .CountAsync(item => item.Status == "ERROR"),
            };

            result.RecentStockMovements =
                await this.databaseContext.StockMovements
                    .AsNoTracking()
                    .OrderByDescending(item => item.CreatedAt)
                    .Take(8)
                    .Select(item => new DashboardStockMovementDto
                    {
                        StockMovementId = item.StockMovementId,

                        MovementType = item.MovementType,

                        Quantity = item.Quantity,

                        LotNumber = item.ProductionLot.LotNumber,

                        ProductCode = item.ProductionLot
                            .ProductionOrder
                            .Product
                            .ProductCode,

                        ProductName = item.ProductionLot
                            .ProductionOrder
                            .Product
                            .Name,

                        Sscc = item.LogisticUnit != null
                            ? item.LogisticUnit.Sscc
                            : null,

                        WarehouseOrderNumber = item.WarehouseOrder != null
                            ? item.WarehouseOrder.OrderNumber
                            : null,

                        CreatedByUserName = item.CreatedByUser.FullName,

                        CreatedAt = item.CreatedAt,
                    })
                    .ToListAsync();

            result.RecentPrintJobs =
                await this.databaseContext.PrintJobs
                    .AsNoTracking()
                    .OrderByDescending(item => item.CreatedAt)
                    .Take(8)
                    .Select(item => new DashboardPrintJobDto
                    {
                        PrintJobId = item.PrintJobId,

                        Status = item.Status,

                        LabelType = item.Label.LabelType,

                        PrimaryCodeValue = item.Label.PrimaryCodeValue,

                        ProductCode = item.Label.Product != null
                            ? item.Label.Product.ProductCode
                            : null,

                        ProductName = item.Label.Product != null
                            ? item.Label.Product.Name
                            : null,

                        PrinterName = item.Printer.Name,

                        Copies = item.Copies,

                        IsReprint = item.IsReprint,

                        CreatedByUserName = item.CreatedByUser.FullName,

                        CreatedAt = item.CreatedAt,

                        ErrorMessage = item.ErrorMessage,
                    })
                    .ToListAsync();

            return result;
        }
    }
}