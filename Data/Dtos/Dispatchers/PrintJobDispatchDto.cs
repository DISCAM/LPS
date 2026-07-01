using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.Dispatchers
{
    public class PrintJobDispatchDto
    {
        public int PrintJobId { get; set; }

        public int LabelId { get; set; }

        public string LabelType { get; set; } = null!;

        public string TemplateReference { get; set; } = null!;

        public int TemplateVersionNo { get; set; }

        public string PrinterName { get; set; } = null!;

        public int Copies { get; set; }

        public bool IsReprint { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public string Ean { get; set; } = null!;

        public string? Gtin { get; set; }
    }
}
