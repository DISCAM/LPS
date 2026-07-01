using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.PrintLabel
{

    
    /// do snapshota label.labeldataJson 
    
    public class PrintEanLabelDataDto
    {
        public int ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public string Ean { get; set; } = null!;

        public string? Gtin { get; set; }

        public int LabelTemplateId { get; set; }

        public string TemplateName { get; set; } = null!;

        public string TemplateReference { get; set; } = null!;

        public int TemplateVersionNo { get; set; }
    }
}
