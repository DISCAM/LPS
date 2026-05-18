using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Table("PrintJobHistory")]
public partial class PrintJobHistory
{
    [Key]
    public int PrintJobHistoryId { get; set; }

    public int PrintJobId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [ForeignKey("PrintJobId")]
    [InverseProperty("PrintJobHistories")]
    public virtual PrintJob PrintJob { get; set; } = null!;
}
