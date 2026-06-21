using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.Printer
{
    public class PrinterDto
    {
        public int PrinterId { get; set; }

        public string Name { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string? Location { get; set; }

        public string? PrinterModel { get; set; }

        public string IntegrationType { get; set; } = null!;

        public bool IsActive { get; set; }

        public int? CreatedByUserId { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
