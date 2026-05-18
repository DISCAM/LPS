using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("Code", Name = "UQ_Permissions_Code", IsUnique = true)]
public partial class Permission
{
    [Key]
    public int PermissionId { get; set; }

    [StringLength(100)]
    public string Code { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("PermissionCreatedByUsers")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("PermissionModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
