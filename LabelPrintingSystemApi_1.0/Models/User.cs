using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? IdentityUserId { get; set; }

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("InverseCreatedByUser")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Customer> CustomerCreatedByUsers { get; set; } = new List<Customer>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<Customer> CustomerModifiedByUsers { get; set; } = new List<Customer>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<User> InverseCreatedByUser { get; set; } = new List<User>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<User> InverseModifiedByUser { get; set; } = new List<User>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<LabelTemplate> LabelTemplateCreatedByUsers { get; set; } = new List<LabelTemplate>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<LabelTemplate> LabelTemplateModifiedByUsers { get; set; } = new List<LabelTemplate>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<LogisticUnit> LogisticUnitCreatedByUsers { get; set; } = new List<LogisticUnit>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<LogisticUnitItem> LogisticUnitItemCreatedByUsers { get; set; } = new List<LogisticUnitItem>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<LogisticUnitItem> LogisticUnitItemModifiedByUsers { get; set; } = new List<LogisticUnitItem>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<LogisticUnit> LogisticUnitModifiedByUsers { get; set; } = new List<LogisticUnit>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("InverseModifiedByUser")]
    public virtual User? ModifiedByUser { get; set; }

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Permission> PermissionCreatedByUsers { get; set; } = new List<Permission>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<Permission> PermissionModifiedByUsers { get; set; } = new List<Permission>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<PrintJob> PrintJobCreatedByUsers { get; set; } = new List<PrintJob>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<PrintJob> PrintJobModifiedByUsers { get; set; } = new List<PrintJob>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Printer> PrinterCreatedByUsers { get; set; } = new List<Printer>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<Printer> PrinterModifiedByUsers { get; set; } = new List<Printer>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<ProductionLot> ProductionLotCreatedByUsers { get; set; } = new List<ProductionLot>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<ProductionLot> ProductionLotModifiedByUsers { get; set; } = new List<ProductionLot>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<ProductionOrder> ProductionOrderCreatedByUsers { get; set; } = new List<ProductionOrder>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<ProductionOrder> ProductionOrderModifiedByUsers { get; set; } = new List<ProductionOrder>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<ReprintRequest> ReprintRequestCreatedByUsers { get; set; } = new List<ReprintRequest>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<ReprintRequest> ReprintRequestModifiedByUsers { get; set; } = new List<ReprintRequest>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Role> RoleCreatedByUsers { get; set; } = new List<Role>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<Role> RoleModifiedByUsers { get; set; } = new List<Role>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<WarehouseOrder> WarehouseOrderCreatedByUsers { get; set; } = new List<WarehouseOrder>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<WarehouseOrderItem> WarehouseOrderItemCreatedByUsers { get; set; } = new List<WarehouseOrderItem>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<WarehouseOrderItem> WarehouseOrderItemModifiedByUsers { get; set; } = new List<WarehouseOrderItem>();

    [InverseProperty("ModifiedByUser")]
    public virtual ICollection<WarehouseOrder> WarehouseOrderModifiedByUsers { get; set; } = new List<WarehouseOrder>();
}
