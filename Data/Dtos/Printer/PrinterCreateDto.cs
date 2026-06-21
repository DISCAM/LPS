using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.Printer
{
    public class PrinterCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; } = null!;

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? PrinterModel { get; set; }

        [Required]
        [StringLength(20)]
        public string IntegrationType { get; set; } = null!;
    }
}
