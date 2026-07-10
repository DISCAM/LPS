using Data.Dtos.LogisticUnits;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface ILogisticUnitsService
    {
        Task<List<LogisticUnitDto>> GetAllAsync();
    }
}
