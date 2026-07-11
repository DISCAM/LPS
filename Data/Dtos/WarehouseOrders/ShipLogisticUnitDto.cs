using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.WarehouseOrders
{
    public class ShipLogisticUnitDto
    {
        [Required]
        public int LogisticUnitId { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }
    }
}
