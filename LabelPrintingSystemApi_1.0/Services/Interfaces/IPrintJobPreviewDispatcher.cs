namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IPrintJobPreviewDispatcher
    {
        Task<string> GetPreviewPathAsync(int printJobId);
    }
}
