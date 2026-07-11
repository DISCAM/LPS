using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.WarehouseOrders
{
    public class CreateWarehouseOrderDto
    {
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; } = null!;

        [MaxLength(20)]
        public string? OrderType { get; set; } = "SHIPMENT";

        [MaxLength(255)]
        public string? DeliveryAddress { get; set; }

        public int? CustomerId { get; set; }
    }
}
