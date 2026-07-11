using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.LogisticUnits
{
    public class LogisticUnitItemDto
    {
        public int LogisticUnitItemId { get; set; }

        public int ProductionLotId { get; set; }

        public string LotNumber { get; set; } = null!;

        public DateOnly ProductionDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public int ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public int ProductionOrderId { get; set; }

        public string ProductionOrderNumber { get; set; } = null!;

        public int? ProductionOrderCustomerId { get; set; }

        public string? ProductionOrderCustomerName { get; set; }

        public decimal Quantity { get; set; }

        public int? WarehouseOrderItemId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
