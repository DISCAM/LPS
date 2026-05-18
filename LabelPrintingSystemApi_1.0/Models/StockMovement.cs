using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class StockMovement
{
    [Key]
    public int StockMovementId { get; set; }

    public int ProductionLotId { get; set; }

    public int? WarehouseOrderId { get; set; }

    public int? LogisticUnitId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string MovementType { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal Quantity { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }

    public int CreatedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("StockMovements")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("LogisticUnitId")]
    [InverseProperty("StockMovements")]
    public virtual LogisticUnit? LogisticUnit { get; set; }

    [ForeignKey("ProductionLotId")]
    [InverseProperty("StockMovements")]
    public virtual ProductionLot ProductionLot { get; set; } = null!;

    [ForeignKey("WarehouseOrderId")]
    [InverseProperty("StockMovements")]
    public virtual WarehouseOrder? WarehouseOrder { get; set; }
}
