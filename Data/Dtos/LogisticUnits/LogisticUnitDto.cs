using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.LogisticUnits
{
    public class LogisticUnitDto
    {
        public int LogisticUnitId { get; set; }

        public string Sscc { get; set; } = null!;

        public string UnitType { get; set; } = null!;

        public string Status { get; set; } = null!;

        public decimal TotalQuantity { get; set; }

        public int? WarehouseOrderId { get; set; }

        public string? WarehouseOrderNumber { get; set; }

        public int CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public List<LogisticUnitItemDto> Items { get; set; } = [];
    }
}
