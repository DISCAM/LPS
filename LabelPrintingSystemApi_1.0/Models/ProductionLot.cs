using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("LotNumber", Name = "UQ_ProductionLots_LotNumber", IsUnique = true)]
public partial class ProductionLot
{
    [Key]
    public int ProductionLotId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string LotNumber { get; set; } = null!;

    public DateOnly ProductionDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    [StringLength(50)]
    public string? ProductionLine { get; set; }

    [StringLength(20)]
    public string? ShiftCode { get; set; }

    public int CreatedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    public int ProductionOrderId { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal ProducedQuantity { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("ProductionLotCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("ProductionLot")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [InverseProperty("ProductionLot")]
    public virtual ICollection<LogisticUnitItem> LogisticUnitItems { get; set; } = new List<LogisticUnitItem>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("ProductionLotModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [ForeignKey("ProductionOrderId")]
    [InverseProperty("ProductionLots")]
    public virtual ProductionOrder ProductionOrder { get; set; } = null!;

    [InverseProperty("ProductionLot")]
    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();

    [InverseProperty("ProductionLot")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
