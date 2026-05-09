using Data;
using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService service;  // pole klasy

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route(Urls.USERS)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            IEnumerable<UserDto> dtos = await service.GetAllUsersAsync();
            return Json(dtos);
        }
        
        [HttpGet]
        [Route(Urls.USERS_ID)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            UserDto dto = await service.GetUserByIdAsync(id);
            return Json(dto);
        }

        [HttpPost]
        [Route(Urls.USERS)]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateDto dto)
        {
            await service.CreateUserAsync(dto);
            return Ok();

        }

        [HttpPut]
        [Route(Urls.USERS_ID)]
        public async Task<IActionResult> EditUserAsync([FromBody] UserEditDto dto, int id)
        {
            if (id != dto.UserId)
            {
                return BadRequest("ID mismatch");
            }
            await service.EditUserAsync(dto);
            return Ok();

        }
        [HttpDelete]
        [Route(Urls.USERS_ID)]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            await service.DeleteCustomerAsync(id);
            return Ok();

        }
    }
}
