using Data.Dtos.Auth;using LabelPrintingSystemApi_1._0.Exceptions;using LabelPrintingSystemApi_1._0.Models;using LabelPrintingSystemApi_1._0.Models.Contexts;using LabelPrintingSystemApi_1._0.Services.Interfaces;using Microsoft.AspNetCore.Identity;using Microsoft.IdentityModel.Tokens;using System.IdentityModel.Tokens.Jwt;using System.Security.Claims;using System.Text;
namespace LabelPrintingSystemApi_1._0.Services{    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DatabaseContext databaseContext;
        private readonly IConfiguration configuration;
        public AuthService(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            DatabaseContext databaseContext,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.databaseContext = databaseContext;
            this.configuration = configuration;
        }

        public async Task RegisterAsync(RegisterUserDto dto)        {            IdentityUser identityUser = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            IdentityResult createUserResult = await userManager.CreateAsync(identityUser, dto.Password);

            if (!createUserResult.Succeeded)
            {
                string errors = string.Join(", ", createUserResult.Errors.Select(error => error.Description));
                throw new BadRequestException(errors);
            }

            string defaultRole = "Operator";

            bool roleExists = await roleManager.RoleExistsAsync(defaultRole);

            if (!roleExists)
            {
                await userManager.DeleteAsync(identityUser);
                throw new BadRequestException($"Role '{defaultRole}' does not exist.");
            }

            IdentityResult addRoleResult = await userManager.AddToRoleAsync(identityUser, defaultRole);

            if (!addRoleResult.Succeeded)
            {
                await userManager.DeleteAsync(identityUser);

                string errors = string.Join(", ", addRoleResult.Errors.Select(error => error.Description));
                throw new BadRequestException(errors);
            }

            User userProfile = new User
            {
                IdentityUserId = identityUser.Id,
                FullName = dto.FullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            databaseContext.Users.Add(userProfile);
            await databaseContext.SaveChangesAsync();        }


        public async Task<LoginResponseDto?> LoginAsync(LoginUserDto dto)        {            IdentityUser? user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)            {                return null;            }
            Microsoft.AspNetCore.Identity.SignInResult result =                await signInManager.CheckPasswordSignInAsync(                    user,                    dto.Password,                    lockoutOnFailure: false);
            if (!result.Succeeded)            {                return null;
            }
            DateTime expiration = DateTime.UtcNow.AddMinutes(
               int.Parse(configuration["Jwt:ExpiresMinutes"]!));
            string token = await GenerateJwtTokenAsync(user, expiration);
            return new LoginResponseDto            {                Token = token,                Email = user.Email!,                Expiration = expiration            };
        }

        public async Task AssignRoleAsync(AssignRoleDto dto)
        {
            IdentityUser? user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new BadRequestException("User not found.");
            }

            bool roleExists = await roleManager.RoleExistsAsync(dto.Role);

            if (!roleExists)
            {
                throw new BadRequestException("Role does not exist.");
            }

            bool userAlreadyInRole = await userManager.IsInRoleAsync(user, dto.Role);

            if (userAlreadyInRole)
            {
                throw new BadRequestException("User already has this role.");
            }

            IdentityResult result = await userManager.AddToRoleAsync(user, dto.Role);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new BadRequestException(errors);
            }
        }


        public async Task CreateDefaultRolesAsync()
        {
            string[] roles =
            {                "Admin",                "Operator",                "Manager"
            };
            foreach (string role in roles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    IdentityRole identityRole = new IdentityRole(role);
                    IdentityResult result = await roleManager.CreateAsync(identityRole);
                    if (!result.Succeeded)
                    {
                        string errors = string.Join(", ", result.Errors.Select(error =>
                                                          error.Description));
                        throw new BadRequestException(errors);
                    }
                }
            }
        }



        private async Task<string> GenerateJwtTokenAsync(IdentityUser user, DateTime expiration)        {            List<Claim> claims = new List<Claim>            {                new Claim(ClaimTypes.NameIdentifier, user.Id),                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())            };
            IList<string> roles = await userManager.GetRolesAsync(user);
            foreach (string role in roles)            {                claims.Add(new Claim(ClaimTypes.Role, role));            }
            string jwtKey = configuration["Jwt:Key"]!;            string jwtIssuer = configuration["Jwt:Issuer"]!;            string jwtAudience = configuration["Jwt:Audience"]!;
            SymmetricSecurityKey key = new SymmetricSecurityKey(                Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials credentials = new SigningCredentials(                key,                SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(                issuer: jwtIssuer,                audience: jwtAudience,                claims: claims,                expires: expiration,                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);        }    }
}
