using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.ProductionLots
{
    public class CreateProductionLotDto
    {
        [Required(ErrorMessage = "Numer LOT jest wymagany.")]
        [StringLength(50)]
        public string LotNumber { get; set; } = null!;

        public DateOnly ProductionDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        [StringLength(50)]
        public string? ProductionLine { get; set; }

        [StringLength(20)]
        public string? ShiftCode { get; set; }

        [Range(typeof(decimal), "0,001", "999999999999999,999",
           ErrorMessage = "Ilość wyprodukowana musi być większa od zera.")]
        public decimal ProducedQuantity { get; set; }
    }
}
