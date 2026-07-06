using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.PrintLabel
{
    public class PrintProductionLabelDataDto : PrintEanLabelDataDto
    {
        public int ProductionLotId { get; set; }

        public int ProductionOrderId { get; set; }

        public string ProductionOrderNumber { get; set; } = null!;

        public string LotNumber { get; set; } = null!;

        public DateOnly ProductionDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public string? ProductionLine { get; set; }

        public string? ShiftCode { get; set; }

        public decimal ProducedQuantity { get; set; }
    }
}
