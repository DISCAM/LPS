using Data;
using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.Auth
{
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;  // pole klasy
        private readonly ILogger<UserController> logger;

        public UserController(IUserService service, ILogger<UserController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        
        [HttpGet]
        [Route(Urls.USERS)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            IEnumerable<UserDto> dtos = await service.GetAllUsersAsync();
            return Ok(dtos);
        }
        


        [HttpGet]
        [Route(Urls.USER_ID)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            UserDto dto = await service.GetUserByIdAsync(id);
            return Ok(dto);
        }

        
    }
}
