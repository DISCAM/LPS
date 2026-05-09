using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext databaseContext;  // pole klasy

        public UserService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            IQueryable<User> quaryable = databaseContext.Users.Where(item => item.IsActive);
            IQueryable<UserDto> gueryableUserDtos = quaryable.Select(item => new UserDto
            {
                UserId = item.UserId,
                Login = item.Login,
                FullName = item.FullName,
                RoleName = item.Role.Name,
                CreatedAt = item.CreatedAt,
                ModifiedAt = item.ModifiedAt
            });
            return await gueryableUserDtos.ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            IQueryable<User> quaryable = databaseContext.Users
                .Where(item => item.UserId == id && item.IsActive);
            IQueryable<UserDto> gueryableUserDtos = quaryable
                .Select(item => new UserDto
            {
                UserId = item.UserId,
                Login = item.Login,
                FullName = item.FullName,
                RoleName = item.Role.Name,
                CreatedAt = item.CreatedAt,
                ModifiedAt = item.ModifiedAt
            });
            return await gueryableUserDtos.FirstOrDefaultAsync() ?? throw new Exception("User not found");
        }

        public async Task CreateUserAsync(UserCreateDto dto)
        {
            User user = new()
            {
                Login = dto.Login,
                FullName = dto.FullName,
                RoleId = dto.RoleId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            databaseContext.Users.Add(user);
            await databaseContext.SaveChangesAsync();


        }

        public async Task EditUserAsync(UserEditDto dto)
        {
            User user = await databaseContext.Users
                .FirstOrDefaultAsync(item => item.UserId == dto.UserId && item.IsActive)
                ?? throw new Exception("Customer not found");
            
            user.Login = dto.Login;
            user.FullName = dto.FullName;
            user.RoleId = dto.RoleId;
            //user.IsActive = true;
            user.ModifiedAt = DateTime.Now;

            await databaseContext.SaveChangesAsync();

        }

        public async Task DeleteCustomerAsync(int id)
        {
            User user = await databaseContext.Users
                .FirstOrDefaultAsync(item => item.UserId == id && item.IsActive)
                ?? throw new Exception("Customer not found");
                        
            user.IsActive = true;
            user.ModifiedAt = DateTime.Now;
            //data usuniecia 
            //kto zmodyfikował usuwał 
            await databaseContext.SaveChangesAsync();

        }

    }
}
