using Data;
using Data.Dtos.AuditLogs;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.AuditLogs
{
    [ApiController]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            this.auditLogService = auditLogService;
        }

        [HttpGet]
        [Route(Urls.AUDIT_LOGS)]
        public async Task<IActionResult> GetAllAuditLogsAsync()
        {
            List<AuditLogDto> result = await this.auditLogService.GetAllAsync();

            return this.Ok(result);
        }
    }
}