
using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext databaseContext;  // pola klasy
        private readonly IdentityContext identityContext;
        private readonly ILogger<UserService> logger;

        public UserService(DatabaseContext databaseContext, IdentityContext identityContext, ILogger<UserService> logger)
        {
            this.databaseContext = databaseContext;
            this.identityContext = identityContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            IQueryable<User> queryable = databaseContext.Users
                .AsNoTracking()
                .Where(item => item.IsActive);

            var users = await queryable
                .Select(item => new
                {
                    item.UserId,
                    item.IdentityUserId,
                    item.FullName,
                    item.CreatedAt,
                    item.ModifiedAt
                })
                .ToListAsync();

            var identityUserIds = users
                .Where(item => item.IdentityUserId != null)
                .Select(item => item.IdentityUserId!)
                .Distinct()
                .ToList();

            var emails = await identityContext.Users
                .AsNoTracking()
                .Where(identityUser => identityUserIds.Contains(identityUser.Id))
                .ToDictionaryAsync(
                    identityUser => identityUser.Id,
                    identityUser => identityUser.Email
                );

            IEnumerable<UserDto> result = users.Select(user => new UserDto
            {
                UserId = user.UserId,
                IdentityUserId = user.IdentityUserId,
                Email = user.IdentityUserId != null &&
                        emails.TryGetValue(user.IdentityUserId, out string? email)
                            ? email
                            : null,
                FullName = user.FullName,
                RoleName = string.Empty,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            });

            return result;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await databaseContext.Users
                .AsNoTracking()
                .Where(item => item.UserId == id && item.IsActive)
                .Select(item => new
        {
            item.UserId,
            item.IdentityUserId,
            item.FullName,
            item.CreatedAt,
            item.ModifiedAt
        })
        .FirstOrDefaultAsync()
        ?? throw new NotFoundException("User not found");

            string? email = null;

            if (user.IdentityUserId != null)
            {
                email = await identityContext.Users
                    .AsNoTracking()
                    .Where(identityUser => identityUser.Id == user.IdentityUserId)
                    .Select(identityUser => identityUser.Email)
                    .FirstOrDefaultAsync();
            }

            return new UserDto
            {
                UserId = user.UserId,
                IdentityUserId = user.IdentityUserId,
                Email = email,
                FullName = user.FullName,
                RoleName = string.Empty,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            };
        }

        


    }
}
