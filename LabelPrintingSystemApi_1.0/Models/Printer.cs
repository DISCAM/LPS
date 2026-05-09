using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("IpAddress", Name = "UQ_Printers_IpAddress", IsUnique = true)]
[Index("Name", Name = "UQ_Printers_Name", IsUnique = true)]
public partial class Printer
{
    [Key]
    public int PrinterId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(45)]
    [Unicode(false)]
    public string IpAddress { get; set; } = null!;

    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(100)]
    public string? PrinterModel { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string IntegrationType { get; set; } = null!;

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [InverseProperty("Printer")]
    public virtual ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();
}
