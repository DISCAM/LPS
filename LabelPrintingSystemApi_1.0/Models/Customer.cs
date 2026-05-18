using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("CustomerCode", Name = "UQ_Customers_CustomerCode", IsUnique = true)]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CustomerCode { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string? TaxNumber { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(150)]
    public string? Street { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("CustomerCreatedByUsers")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("CustomerModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();

    [InverseProperty("Customer")]
    public virtual ICollection<WarehouseOrder> WarehouseOrders { get; set; } = new List<WarehouseOrder>();
}
