using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class Label
{
    [Key]
    public int LabelId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string LabelType { get; set; } = null!;

    public int? ProductId { get; set; }

    public int? ProductionLotId { get; set; }

    public int? LogisticUnitId { get; set; }

    public int LabelTemplateId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? PrimaryCodeValue { get; set; }

    public string? LabelDataJson { get; set; }

    public int CreatedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("Labels")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("LabelTemplateId")]
    [InverseProperty("Labels")]
    public virtual LabelTemplate LabelTemplate { get; set; } = null!;

    [ForeignKey("LogisticUnitId")]
    [InverseProperty("Labels")]
    public virtual LogisticUnit? LogisticUnit { get; set; }

    [InverseProperty("Label")]
    public virtual ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();

    [ForeignKey("ProductId")]
    [InverseProperty("Labels")]
    public virtual Product? Product { get; set; }

    [ForeignKey("ProductionLotId")]
    [InverseProperty("Labels")]
    public virtual ProductionUnit? ProductionLot { get; set; }

    [InverseProperty("SourceLabel")]
    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();
}
