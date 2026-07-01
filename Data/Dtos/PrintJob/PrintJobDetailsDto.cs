using Data.Dtos.PrintLabel;


namespace Data.Dtos.PrintJob
{
    public class PrintJobDetailsDto
    {
        public int PrintJobId { get; set; }

        public int LabelId { get; set; }

        public string LabelType { get; set; } = null!;

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public string? PrimaryCodeValue { get; set; }

        public string TemplateName { get; set; } = null!;

        public string PrinterName { get; set; } = null!;

        public int Copies { get; set; }

        public string Status { get; set; } = null!;

        public bool IsReprint { get; set; }

        public string? ErrorMessage { get; set; }

        public string? CreatedByUserName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ModifiedByUserName { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public PrintEanLabelDataDto? LabelData { get; set; }
        public List<PrintJobHistoryDto> History { get; set; } = [];
    }
}