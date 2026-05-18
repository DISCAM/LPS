using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("OrderNumber", Name = "UQ_ProductionOrders_OrderNumber", IsUnique = true)]
public partial class ProductionOrder
{
    [Key]
    public int ProductionOrderId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string OrderNumber { get; set; } = null!;

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal PlannedQuantity { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public DateOnly? PlannedStartDate { get; set; }

    public DateOnly? PlannedEndDate { get; set; }

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    public int? CustomerId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ProductionOrderType { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("ProductionOrderCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("ProductionOrders")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("ProductionOrderModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductionOrders")]
    public virtual Product Product { get; set; } = null!;

    [InverseProperty("ProductionOrder")]
    public virtual ICollection<ProductionLot> ProductionLots { get; set; } = new List<ProductionLot>();
}
