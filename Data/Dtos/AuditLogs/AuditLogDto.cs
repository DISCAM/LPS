using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos.AuditLogs
{
    public class AuditLogDto
    {
        public int AuditLogId { get; set; }

        public int? CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; } = null!;

        public string EntityName { get; set; } = null!;

        public int? EntityId { get; set; }

        public string Action { get; set; } = null!;

        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
