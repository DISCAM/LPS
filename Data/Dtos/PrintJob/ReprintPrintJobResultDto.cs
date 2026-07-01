using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.PrintJob
{
    public class ReprintPrintJobResultDto
    {
        public int PrintJobId { get; set; }

        public int LabelId { get; set; }

        public string Status { get; set; } = null!;

        public bool IsReprint { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
