using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.ProductionLots
{
    public class ProductionLotDto
    {
        public int ProductionLotId { get; set; }

        public int ProductionOrderId { get; set; }

        public string ProductionOrderNumber { get; set; } = null!;

        public int ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public string LotNumber { get; set; } = null!;

        public DateOnly ProductionDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public string? ProductionLine { get; set; }

        public string? ShiftCode { get; set; }

        public decimal ProducedQuantity { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
