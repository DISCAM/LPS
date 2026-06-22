using Data.Dtos.LabelTemplate;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface ILabelTemplateService
    {
        Task CreateLabelTemplateAsync(LabelTemplateCreateDto dto);
        Task DeleteLabelTemplateAsync(int id);
        Task EditLabelTemplateAsync(LabelTemplateEditDto dto);
        Task<IEnumerable<LabelTemplateDto>> GetAllLabelTemplatesAsync();
        Task<LabelTemplateDto> GetLabelTemplateByIdAsync(int id);
    }
}