using Data.Dtos.LabelTemplate;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LabelPrintingSystemApi_1._0.Services.Konfiguracja
{
    public class LabelTemplateService : ILabelTemplateService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<LabelTemplateService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LabelTemplateService(
            DatabaseContext databaseContext,
            ILogger<LabelTemplateService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<LabelTemplateDto>> GetAllLabelTemplatesAsync()
        {
            IQueryable<LabelTemplate> queryable = databaseContext.LabelTemplates
                .AsNoTracking()
                .Where(item => item.IsActive);

            return await queryable
                .Select(item => new LabelTemplateDto
                {
                    LabelTemplateId = item.LabelTemplateId,
                    Name = item.Name,
                    LabelType = item.LabelType,
                    TemplateEngine = item.TemplateEngine,
                    TemplateReference = item.TemplateReference,
                    VersionNo = item.VersionNo,
                    IsDefault = item.IsDefault,
                    IsActive = item.IsActive,
                    CreatedByUserId = item.CreatedByUserId,
                    ModifiedByUserId = item.ModifiedByUserId,
                    CreatedAt = item.CreatedAt,
                    ModifiedAt = item.ModifiedAt
                })
                .ToListAsync();
        }

        public async Task<LabelTemplateDto> GetLabelTemplateByIdAsync(int id)
        {
            LabelTemplate labelTemplate = await databaseContext.LabelTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                    item.LabelTemplateId == id &&
                    item.IsActive)
                ?? throw new NotFoundException("Label template not found");

            return new LabelTemplateDto
            {
                LabelTemplateId = labelTemplate.LabelTemplateId,
                Name = labelTemplate.Name,
                LabelType = labelTemplate.LabelType,
                TemplateEngine = labelTemplate.TemplateEngine,
                TemplateReference = labelTemplate.TemplateReference,
                VersionNo = labelTemplate.VersionNo,
                IsDefault = labelTemplate.IsDefault,
                IsActive = labelTemplate.IsActive,
                CreatedByUserId = labelTemplate.CreatedByUserId,
                ModifiedByUserId = labelTemplate.ModifiedByUserId,
                CreatedAt = labelTemplate.CreatedAt,
                ModifiedAt = labelTemplate.ModifiedAt
            };
        }

        public async Task CreateLabelTemplateAsync(LabelTemplateCreateDto dto)
        {
            int currentUserId = await GetCurrentUserIdAsync();
            LabelTemplate labelTemplate = new LabelTemplate
            {
                Name = dto.Name,
                LabelType = dto.LabelType,
                TemplateEngine = dto.TemplateEngine,
                TemplateReference = dto.TemplateReference,
                VersionNo = dto.VersionNo,
                IsDefault = dto.IsDefault,
                IsActive = true,
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            databaseContext.LabelTemplates.Add(labelTemplate);

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                $"Label template was created. LabelTemplateId: {labelTemplate.LabelTemplateId}");
        }

        public async Task EditLabelTemplateAsync(LabelTemplateEditDto dto)
        {
            int currentUserId = await GetCurrentUserIdAsync();

            LabelTemplate labelTemplate = await databaseContext.LabelTemplates
                .FirstOrDefaultAsync(item =>
                    item.LabelTemplateId == dto.LabelTemplateId &&
                    item.IsActive)
                ?? throw new NotFoundException("Label template not found");

            labelTemplate.Name = dto.Name;
            labelTemplate.LabelType = dto.LabelType;
            labelTemplate.TemplateEngine = dto.TemplateEngine;
            labelTemplate.TemplateReference = dto.TemplateReference;
            labelTemplate.VersionNo = dto.VersionNo;
            labelTemplate.IsDefault = dto.IsDefault;
            labelTemplate.ModifiedByUserId = currentUserId;
            labelTemplate.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                $"Label template was edited. LabelTemplateId: {labelTemplate.LabelTemplateId}");
        }

        public async Task DeleteLabelTemplateAsync(int id)
        {
            int currentUserId = await GetCurrentUserIdAsync();

            LabelTemplate labelTemplate = await databaseContext.LabelTemplates
                .FirstOrDefaultAsync(item =>
                    item.LabelTemplateId == id &&
                    item.IsActive)
                ?? throw new NotFoundException("Label template not found");

            labelTemplate.IsActive = false;
            labelTemplate.IsDefault = false;
            labelTemplate.ModifiedByUserId = currentUserId;
            labelTemplate.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                $"Label template was deactivated. LabelTemplateId: {labelTemplate.LabelTemplateId}");
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            string? identityUserId = httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            User user = await databaseContext.Users
                .FirstOrDefaultAsync(item =>
                    item.IdentityUserId == identityUserId &&
                    item.IsActive)
                ?? throw new NotFoundException("Active system user not found");

            return user.UserId;
        }
    }
}