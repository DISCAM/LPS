using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Dtos.PrintLabel
{
    public class PrintEanDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int LabelTemplateId { get; set; }

        [Range(1, int.MaxValue)]
        public int PrinterId { get; set; }

        [Range(1, 1000)]
        public int Copies { get; set; }

    }
}
