using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class WarehouseOrderItem
{
    [Key]
    public int WarehouseOrderItemId { get; set; }

    public int WarehouseOrderId { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal RequiredQuantity { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal PickedQuantity { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("WarehouseOrderItemCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("WarehouseOrderItem")]
    public virtual ICollection<LogisticUnitItem> LogisticUnitItems { get; set; } = new List<LogisticUnitItem>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("WarehouseOrderItemModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("WarehouseOrderItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("WarehouseOrderId")]
    [InverseProperty("WarehouseOrderItems")]
    public virtual WarehouseOrder WarehouseOrder { get; set; } = null!;
}
