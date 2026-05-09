using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models;

public partial class LabelTemplate
{
    [Key]
    public int LabelTemplateId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string LabelType { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string TemplateEngine { get; set; } = null!;

    [StringLength(255)]
    public string TemplateReference { get; set; } = null!;

    public int VersionNo { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("LabelTemplateCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("LabelTemplate")]
    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("LabelTemplateModifiedByUsers")]
    public virtual User? ModifiedByUser { get; set; }
}
