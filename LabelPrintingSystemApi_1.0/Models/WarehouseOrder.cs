using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("OrderNumber", Name = "UQ_WarehouseOrders_OrderNumber", IsUnique = true)]
public partial class WarehouseOrder
{
    [Key]
    public int WarehouseOrderId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string OrderNumber { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string OrderType { get; set; } = null!;

    [StringLength(255)]
    public string? DeliveryAddress { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    public int? CustomerId { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("WarehouseOrderCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("WarehouseOrders")]
    public virtual Customer? Customer { get; set; }

    [InverseProperty("WarehouseOrder")]
    public virtual ICollection<LogisticUnit> LogisticUnits { get; set; } = new List<LogisticUnit>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("WarehouseOrderModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [InverseProperty("WarehouseOrder")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    [InverseProperty("WarehouseOrder")]
    public virtual ICollection<WarehouseOrderItem> WarehouseOrderItems { get; set; } = new List<WarehouseOrderItem>();
}
