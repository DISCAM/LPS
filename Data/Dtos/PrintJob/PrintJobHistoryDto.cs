using System;

namespace Data.Dtos.PrintJob
{
    public class PrintJobHistoryDto
    {
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public string? Note { get; set; }
    }
}