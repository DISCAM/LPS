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

    public int CreatedByUserId { get; set; }

    [StringLength(255)]
    public string? Reason { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("ReprintRequestCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("ReprintRequestModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [ForeignKey("PrintJobId")]
    [InverseProperty("ReprintRequests")]
    public virtual PrintJob PrintJob { get; set; } = null!;
}
