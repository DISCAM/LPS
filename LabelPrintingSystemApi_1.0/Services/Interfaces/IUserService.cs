using Data.Dtos.User;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IUserService
    {
        
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
    }
}
