using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.ProductionOrders
{
    public class ProductionOrderDto
    {
        public int ProductionOrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public int ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public decimal PlannedQuantity { get; set; }

        public string Status { get; set; } = null!;

        public DateOnly? PlannedStartDate { get; set; }

        public DateOnly? PlannedEndDate { get; set; }

        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string ProductionOrderType { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
