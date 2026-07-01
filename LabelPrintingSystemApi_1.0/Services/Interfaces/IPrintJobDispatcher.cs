namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IPrintJobDispatcher
    {
        Task DispatchPrintJobAsync(int printJobId);
    }
}
