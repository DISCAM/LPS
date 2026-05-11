using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext databaseContext;  // pole klasy
        private readonly IMapper mapper;

        public UserService(DatabaseContext databaseContext, IMapper mapper)
        {
            this.databaseContext = databaseContext;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            IQueryable<User> queryable = databaseContext.Users.Where(item => item.IsActive);

            //IQueryable<UserDto> gueryableUserDtos = quaryable.Select(item => new UserDto
            //{
            //    UserId = item.UserId,
            //    Login = item.Login,
            //    FullName = item.FullName,
            //    RoleName = item.Role.Name,
            //    CreatedAt = item.CreatedAt,
            //    ModifiedAt = item.ModifiedAt
            //});
            
            // rozbimy zamist select używamy automapera 

            IQueryable<UserDto> projectedQuery = queryable
                .ProjectTo<UserDto>(mapper.ConfigurationProvider);

            //List<UserDto> userDtos = await projectedQuery.ToListAsync();

            return await projectedQuery.ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            IQueryable<User> quaryable = databaseContext.Users
                .Where(item => item.UserId == id && item.IsActive);

            //IQueryable<UserDto> gueryableUserDtos = quaryable
            //    .Select(item => new UserDto
            //{
            //    UserId = item.UserId,
            //    Login = item.Login,
            //    FullName = item.FullName,
            //    RoleName = item.Role.Name,
            //    CreatedAt = item.CreatedAt,
            //    ModifiedAt = item.ModifiedAt
            //});

            IQueryable<UserDto> gueryableUserDtos = quaryable
                .ProjectTo<UserDto>(mapper.ConfigurationProvider);

            return await gueryableUserDtos.FirstOrDefaultAsync() ?? throw new Exception("User not found");
        }

        public async Task CreateUserAsync(UserCreateDto dto)
        {
            User user = mapper.Map<User>(dto);

            // User user = new()
            // {s
            //     Login = dto.Login,
            //     FullName = dto.FullName,
            //     RoleId = dto.RoleId,
            //     IsActive = true,
            //     CreatedAt = DateTime.Now
            // };

            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.PasswordHash = "$$%%**#";

            databaseContext.Users.Add(user);
            await databaseContext.SaveChangesAsync();


        }

        public async Task EditUserAsync(UserEditDto dto)
        {
            User user = await databaseContext.Users
                .FirstOrDefaultAsync(item => item.UserId == dto.UserId && item.IsActive)
                ?? throw new Exception("Customer not found");

            mapper.Map(dto, user);

            //user.Login = dto.Login;
            //user.FullName = dto.FullName;
            //user.RoleId = dto.RoleId;
            //user.IsActive = true;
            user.ModifiedAt = DateTime.Now;
              

            await databaseContext.SaveChangesAsync();

        }

        public async Task DeleteCustomerAsync(int id)
        {
            User user = await databaseContext.Users
                .FirstOrDefaultAsync(item => item.UserId == id && item.IsActive)
                ?? throw new Exception("Customer not found");
                        
            user.IsActive = false;
            user.ModifiedAt = DateTime.Now;
            //data usuniecia 
            //kto zmodyfikował usuwał 
            await databaseContext.SaveChangesAsync();

        }

    }
}
