using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.WarehouseOrders
{
    public class WarehouseOrderDetailsDto
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

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public List<WarehouseOrderLogisticUnitDto> LogisticUnits { get; set; } =
            new List<WarehouseOrderLogisticUnitDto>();
    }

    public class WarehouseOrderLogisticUnitDto
    {
        public int LogisticUnitId { get; set; }

        public string Sscc { get; set; } = null!;

        public string UnitType { get; set; } = null!;

        public string Status { get; set; } = null!;

        public decimal TotalQuantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<WarehouseOrderLogisticUnitItemDto> Items { get; set; } =
            new List<WarehouseOrderLogisticUnitItemDto>();
    }

    public class WarehouseOrderLogisticUnitItemDto
    {
        public int LogisticUnitItemId { get; set; }

        public int ProductionLotId { get; set; }

        public string LotNumber { get; set; } = null!;

        public int ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public decimal Quantity { get; set; }
    }
}
