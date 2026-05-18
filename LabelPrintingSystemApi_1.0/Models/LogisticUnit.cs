using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("Sscc", Name = "UQ_LogisticUnits_SSCC", IsUnique = true)]
public partial class LogisticUnit
{
    [Key]
    public int LogisticUnitId { get; set; }

    [Column("SSCC")]
    [StringLength(18)]
    [Unicode(false)]
    public string Sscc { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string UnitType { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    public int? WarehouseOrderId { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("LogisticUnitCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("LogisticUnit")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [InverseProperty("LogisticUnit")]
    public virtual ICollection<LogisticUnitItem> LogisticUnitItems { get; set; } = new List<LogisticUnitItem>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("LogisticUnitModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [InverseProperty("LogisticUnit")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    [ForeignKey("WarehouseOrderId")]
    [InverseProperty("LogisticUnits")]
    public virtual WarehouseOrder? WarehouseOrder { get; set; }
}
