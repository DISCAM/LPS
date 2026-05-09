using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("Login", Name = "UQ_Users_Login", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string Login { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<LabelTemplate> LabelTemplateCreatedByUsers { get; set; } = new List<LabelTemplate>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<LabelTemplate> LabelTemplateModifiedByUsers { get; set; } = new List<LabelTemplate>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<LogisticUnit> LogisticUnits { get; set; } = new List<LogisticUnit>();

    [InverseProperty("RequestedByUser")]
    public virtual ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<ProductionUnit> ProductionUnits { get; set; } = new List<ProductionUnit>();

    [InverseProperty("RequestedByUser")]
    public virtual ICollection<ReprintRequest> ReprintRequests { get; set; } = new List<ReprintRequest>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();
}
