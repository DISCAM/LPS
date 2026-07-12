using Data.Dtos.AuditLogs;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<List<AuditLogDto>> GetAllAsync();

        Task AddAsync(int? createdByUserId, string entityName, int? entityId, string action, string? details);
    }
}