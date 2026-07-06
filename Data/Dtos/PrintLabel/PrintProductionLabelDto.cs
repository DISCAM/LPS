using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.PrintLabel
{
    public class PrintProductionLabelDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Partia produkcyjna jest wymagana.")]
        public int ProductionLotId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Szablon etykiety jest wymagany.")]
        public int LabelTemplateId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Drukarka jest wymagana.")]
        public int PrinterId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Liczba kopii musi być większa od zera.")]
        public int Copies { get; set; }
    }
}
