using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.PrintLabel
{
    public class PrintLogisticLabelDataDto : PrintEanLabelDataDto
    {
        public int LogisticUnitId { get; set; }

        public string Sscc { get; set; } = null!;

        public string UnitType { get; set; } = null!;

        public int ProductionLotId { get; set; }

        public string LotNumber { get; set; } = null!;

        public DateOnly ProductionDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public decimal Quantity { get; set; }
    }
}
