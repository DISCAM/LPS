using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.PrintLabel
{
    public class PrintResultDto
    {
        public int LabelId { get; set; }

        public int PrintJobId { get; set; }

        public string LabelType { get; set; } = null!;

        public string PrimaryCodeValue { get; set; } = null!;

        public int LabelTemplateId { get; set; }

        public string TemplateName { get; set; } = null!;

        public int PrinterId { get; set; }

        public string PrinterName { get; set; } = null!;

        public int CreatedByUserId { get; set; }

        public int Copies { get; set; }

        public string Status { get; set; } = null!;

        public bool IsReprint { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
