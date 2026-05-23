using Data;
using Data.Dtos.Auth;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LabelPrintingSystemApi_1._0.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        [HttpPost]
        [Route(Urls.REGISTER)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto dto)
        {
            await authService.RegisterAsync(dto);
            return Ok("User registered successfully");
        }
        [HttpPost]
        [Route(Urls.LOGIN)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserDto dto)
        {
            LoginResponseDto? response = await authService.LoginAsync(dto);
            if (response == null)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(response);
        }

        [HttpPost]
        [Route(Urls.CDR)]
        public async Task<IActionResult> CreateDefaultRolesAsync()
        {
            await authService.CreateDefaultRolesAsync();
            return Ok("Default roles created successfully");
        }

        [HttpPost]
        [Route(Urls.AR)]
        public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleDto dto)
        {
            await authService.AssignRoleAsync(dto);
            return Ok("Role assigned successfully");
        }
        
        [Authorize]
        [HttpGet]
        [Route("me")]
        public IActionResult Me()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? email = User.FindFirstValue(ClaimTypes.Email);
            string? userName = User.FindFirstValue(ClaimTypes.Name);
            List<string> roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToList();
            return Ok(new
            {
                UserId = userId,
                Email = email,
                UserName = userName,
                Roles = roles
            });
        }
        
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("admin-test")]
        public IActionResult AdminTest()
        {
            return Ok("You are Admin. Access granted.");
        }


    }
}


