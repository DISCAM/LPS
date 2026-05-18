using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class LogisticUnitItem
{
    [Key]
    public int LogisticUnitItemId { get; set; }

    public int LogisticUnitId { get; set; }

    public int? WarehouseOrderItemId { get; set; }

    public int ProductionLotId { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal Quantity { get; set; }

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("LogisticUnitItemCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("LogisticUnitId")]
    [InverseProperty("LogisticUnitItems")]
    public virtual LogisticUnit LogisticUnit { get; set; } = null!;

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("LogisticUnitItemModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [ForeignKey("ProductionLotId")]
    [InverseProperty("LogisticUnitItems")]
    public virtual ProductionLot ProductionLot { get; set; } = null!;

    [ForeignKey("WarehouseOrderItemId")]
    [InverseProperty("LogisticUnitItems")]
    public virtual WarehouseOrderItem? WarehouseOrderItem { get; set; }
}
