using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.WarehouseReceipts
{
    public class WarehouseReceiptResultDto
    {
        public int LogisticUnitId { get; set; }

        public string Sscc { get; set; } = null!;

        public int LogisticUnitItemId { get; set; }

        public int StockMovementId { get; set; }

        public int ProductionLotId { get; set; }

        public string LotNumber { get; set; } = null!;

        public decimal Quantity { get; set; }

        public string UnitType { get; set; } = null!;

        public string MovementType { get; set; } = null!;

        public string Status { get; set; } = null!;

        public int LabelId { get; set; }

        public int PrintJobId { get; set; }

        public string PrintJobStatus { get; set; } = null!;
    }
}
