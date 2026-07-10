using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.StockMovements
{
    public class StockMovementDto
    {
        public int StockMovementId { get; set; }

        public string MovementType { get; set; } = null!;

        public decimal Quantity { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ProductionLotId { get; set; }

        public string LotNumber { get; set; } = null!;

        public DateOnly ProductionDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public int ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public int? LogisticUnitId { get; set; }

        public string? Sscc { get; set; }

        public string? UnitType { get; set; }

        public int? WarehouseOrderId { get; set; }

        public string? WarehouseOrderNumber { get; set; }

        public int CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; } = null!;
    }
}
