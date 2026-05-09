using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

[Index("ProductCode", Name = "UQ_Products_ProductCode", IsUnique = true)]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ProductCode { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Column("EAN")]
    [StringLength(14)]
    [Unicode(false)]
    public string? Ean { get; set; }

    [Column("GTIN")]
    [StringLength(14)]
    [Unicode(false)]
    public string? Gtin { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductionUnit> ProductionUnits { get; set; } = new List<ProductionUnit>();
}
