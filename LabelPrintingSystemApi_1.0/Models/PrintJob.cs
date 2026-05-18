using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class PrintJob
{
    [Key]
    public int PrintJobId { get; set; }

    public int LabelId { get; set; }

    public int PrinterId { get; set; }

    public int CreatedByUserId { get; set; }

    public int? ScanEventId { get; set; }

    public int Copies { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public bool IsReprint { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("PrintJobCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("LabelId")]
    [InverseProperty("PrintJobs")]
    public virtual Label Label { get; set; } = null!;

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("PrintJobModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [InverseProperty("PrintJob")]
    public virtual ICollection<PrintJobHistory> PrintJobHistories { get; set; } = new List<PrintJobHistory>();

    [ForeignKey("PrinterId")]
    [InverseProperty("PrintJobs")]
    public virtual Printer Printer { get; set; } = null!;

    [InverseProperty("PrintJob")]
    public virtual ICollection<ReprintRequest> ReprintRequests { get; set; } = new List<ReprintRequest>();

    [ForeignKey("ScanEventId")]
    [InverseProperty("PrintJobs")]
    public virtual ScanEvent? ScanEvent { get; set; }
}
