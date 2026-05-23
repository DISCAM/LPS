using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[PrimaryKey("RoleId", "PermissionId")]
public partial class RolePermission
{
    [Key]
    public int PermissionId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    [Key]
    public string RoleId { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("RolePermissions")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("RolePermissions")]
    public virtual Permission Permission { get; set; } = null!;
}
