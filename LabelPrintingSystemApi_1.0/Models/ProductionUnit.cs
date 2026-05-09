using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("LotNumber", Name = "UQ_ProductionLots_LotNumber", IsUnique = true)]
public partial class ProductionUnit
{
    [Key]
    public int ProductionUnitId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string LotNumber { get; set; } = null!;

    public int ProductId { get; set; }

    public DateOnly ProductionDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    [StringLength(50)]
    public string? ProductionLine { get; set; }

    [StringLength(20)]
    public string? ShiftCode { get; set; }

    public int CreatedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("ProductionUnits")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("ProductionLot")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [InverseProperty("ProductionLot")]
    public virtual ICollection<LogisticUnit> LogisticUnits { get; set; } = new List<LogisticUnit>();

    [ForeignKey("ProductId")]
    [InverseProperty("ProductionUnits")]
    public virtual Product Product { get; set; } = null!;

    [InverseProperty("ProductionUnit")]
    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();
}
