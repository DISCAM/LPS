using Data.Dtos.PrintLabel;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IPrintLabelService
    {
        Task<PrintResultDto> PrintEanAsync(PrintEanDto dto, string identityUserId);
    }
}