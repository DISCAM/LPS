using Data.Dtos.AuditLogs;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.AuditLogs
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DatabaseContext databaseContext;

        public AuditLogService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<AuditLogDto>> GetAllAsync()
        {
            return await this.databaseContext.AuditLogs
                .AsNoTracking()
                .OrderByDescending(item => item.CreatedAt)
                .Take(300)
                .Select(item => new AuditLogDto
                {
                    AuditLogId = item.AuditLogId,
                    CreatedByUserId = item.CreatedByUserId,
                    CreatedByUserName = item.CreatedByUser != null
                        ? item.CreatedByUser.FullName
                        : "System",

                    EntityName = item.EntityName,
                    EntityId = item.EntityId,
                    Action = item.Action,
                    Details = item.Details,
                    CreatedAt = item.CreatedAt,
                }).ToListAsync();
        }

        public async Task AddAsync(int? createdByUserId, string entityName, int? entityId, string action, string? details)
        {
            AuditLog auditLog = new()
            {
                CreatedByUserId = createdByUserId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Details = details,
                CreatedAt = DateTime.Now,
            };

            await this.databaseContext.AuditLogs.AddAsync(auditLog);
        }
    }
}