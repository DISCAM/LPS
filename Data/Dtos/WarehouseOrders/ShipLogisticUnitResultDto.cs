using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.WarehouseOrders
{
    public class ShipLogisticUnitResultDto
    {
        public int WarehouseOrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public string WarehouseOrderStatus { get; set; } = null!;

        public int LogisticUnitId { get; set; }

        public string Sscc { get; set; } = null!;

        public string LogisticUnitStatus { get; set; } = null!;

        public decimal Quantity { get; set; }

        public string MovementType { get; set; } = null!;

        public List<int> StockMovementIds { get; set; } =
            new List<int>();
    }
}
