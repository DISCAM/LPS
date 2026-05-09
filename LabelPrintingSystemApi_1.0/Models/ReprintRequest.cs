using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class ReprintRequest
{
    [Key]
    public int ReprintRequestId { get; set; }

    public int PrintJobId { get; set; }

    public int RequestedByUserId { get; set; }

    [StringLength(255)]
    public string? Reason { get; set; }

    [Precision(0)]
    public DateTime RequestedAt { get; set; }

    [ForeignKey("PrintJobId")]
    [InverseProperty("ReprintRequests")]
    public virtual PrintJob PrintJob { get; set; } = null!;

    [ForeignKey("RequestedByUserId")]
    [InverseProperty("ReprintRequests")]
    public virtual User RequestedByUser { get; set; } = null!;
}
