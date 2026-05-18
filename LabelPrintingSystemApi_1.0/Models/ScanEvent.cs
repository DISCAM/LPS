using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class ScanEvent
{
    [Key]
    public int ScanEventId { get; set; }

    public int CreatedByUserId { get; set; }

    public int? SourceLabelId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string ScannedValue { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string ScanType { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string ResultStatus { get; set; } = null!;

    public int? ProductionLotId { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("ScanEvents")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("ScanEvent")]
    public virtual ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();

    [ForeignKey("ProductionLotId")]
    [InverseProperty("ScanEvents")]
    public virtual ProductionLot? ProductionLot { get; set; }

    [ForeignKey("SourceLabelId")]
    [InverseProperty("ScanEvents")]
    public virtual Label? SourceLabel { get; set; }
}
