using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.WarehouseReceipts
{
    public class CreateWarehouseReceiptDto
    {
        [Required]
        public int ProductionLotId { get; set; }

        [Required]
        [Range(typeof(decimal), "0,001", "999999999999999,999", ErrorMessage = "Ilość musi być większa od 0.")]
        public decimal Quantity { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnitType { get; set; } = null!;

        [MaxLength(255)]
        public string? Notes { get; set; }

        [Required]
        public int LabelTemplateId { get; set; }

        [Required]
        public int PrinterId { get; set; }

        [Range(1, 999, ErrorMessage = "Liczba kopii musi być większa od 0.")]
        public int Copies { get; set; } = 1;
    }
}
