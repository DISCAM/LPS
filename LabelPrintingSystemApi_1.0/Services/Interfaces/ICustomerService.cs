using Data.Dtos.Customer;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface ICustomerService
    {
        Task CreateCustomerAsync(CustomerCreateDto dto);
        Task DeleteCustomerAsync(int id);
        Task EditCustomerAsync(CustomerEditDto dto);
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto> GetCustomerByIdAsync(int id);
    }
}