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

    public int ProductionLotId { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? Quantity { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("LogisticUnits")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("LogisticUnit")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [ForeignKey("ProductionLotId")]
    [InverseProperty("LogisticUnits")]
    public virtual ProductionUnit ProductionLot { get; set; } = null!;
}
