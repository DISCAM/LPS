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

    [ForeignKey("PrintJobId")]
    [InverseProperty("PrintJobHistories")]
    public virtual PrintJob PrintJob { get; set; } = null!;
}
