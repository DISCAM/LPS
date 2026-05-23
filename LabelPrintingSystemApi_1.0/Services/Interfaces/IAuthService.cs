using Data.Dtos.Auth;
namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginUserDto dto);
        Task RegisterAsync(RegisterUserDto dto);
        Task CreateDefaultRolesAsync();
        Task AssignRoleAsync(AssignRoleDto dto);
    }
}

