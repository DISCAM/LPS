using Data.Dtos.User;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(UserCreateDto dto);
        Task DeleteCustomerAsync(int id);
        Task EditUserAsync(int id, UserEditDto dto);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
    }
}
