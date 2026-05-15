using Data;
using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers
{
    [ApiController]
    public class UserController : BaseController
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
            return Json(dtos);
        }
        
        [HttpGet]
        [Route(Urls.USER_ID)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            UserDto dto = await service.GetUserByIdAsync(id);
            return Json(dto);
        }

        [HttpPost]
        [Route(Urls.USER)]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateDto dto)
        {
            
            await service.CreateUserAsync(dto);
            return Ok();

        }

        [HttpPut]
        [Route(Urls.USER_ID)]
        public async Task<IActionResult> EditUserAsync([FromBody] UserEditDto dto, int id)
        {
            // przenosimy to sprawdzanie do exceptions  
            //if (id != dto.UserId)
            //{
            //    return BadRequest("ID mismatch");
            //}
            await service.EditUserAsync(id, dto);
            return Ok();

        }
        [HttpDelete]
        [Route(Urls.USER_ID)]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            await service.DeleteCustomerAsync(id);
            return Ok();

        }
    }
}
