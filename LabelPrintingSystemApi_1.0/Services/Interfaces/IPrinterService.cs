using Data.Dtos.Printer;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IPrinterService
    {
        Task CreatePrinterAsync(PrinterCreateDto dto);
        Task DeletePrinterAsync(int id);
        Task EditPrinterAsync(PrinterEditDto dto);
        Task<IEnumerable<PrinterDto>> GetAllPrintersAsync();
        Task<PrinterDto> GetPrinterByIdAsync(int id);
    }
}