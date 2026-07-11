using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.WarehouseOrders
{
    public class WarehouseOrderDto
    {
        public int WarehouseOrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public string OrderType { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? DeliveryAddress { get; set; }

        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public int CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int LogisticUnitsCount { get; set; }

        public decimal TotalQuantity { get; set; }
    }
}
