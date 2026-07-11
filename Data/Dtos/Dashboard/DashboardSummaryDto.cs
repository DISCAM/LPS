using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.Dashboard
{
    public class DashboardSummaryDto
    {
        public int ActiveProductionOrdersCount { get; set; }
        public int ProductionLotsTodayCount { get; set; }
        public int LogisticUnitsInStockCount { get; set; }
        public int LogisticUnitsShippedCount { get; set; }
        public int WarehouseOrdersNewCount { get; set; }
        public int WarehouseOrdersCompletedCount { get; set; }
        public int PrintJobsQueuedCount { get; set; }
        public int PrintJobsSentCount { get; set; }
        public int PrintJobsErrorCount { get; set; }
        public List<DashboardStockMovementDto> RecentStockMovements { get; set; } =
            new List<DashboardStockMovementDto>();
        public List<DashboardPrintJobDto> RecentPrintJobs { get; set; } =
            new List<DashboardPrintJobDto>();
    }

    public class DashboardStockMovementDto
    {
        public int StockMovementId { get; set; }
        public string MovementType { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string LotNumber { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Sscc { get; set; }
        public string? WarehouseOrderNumber { get; set; }
        public string CreatedByUserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardPrintJobDto
    {
        public int PrintJobId { get; set; }
        public string Status { get; set; } = null!;
        public string LabelType { get; set; } = null!;
        public string? PrimaryCodeValue { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string PrinterName { get; set; } = null!;
        public int Copies { get; set; }
        public bool IsReprint { get; set; }
        public string CreatedByUserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
