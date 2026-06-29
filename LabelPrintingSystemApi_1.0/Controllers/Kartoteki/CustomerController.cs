using Data;
using Data.Dtos.Customer;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.Kartoteki
{
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet]
        [Route(Urls.CUSTOMERS)]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            IEnumerable<CustomerDto> customers = await customerService.GetAllCustomersAsync();

            return Ok(customers);
        }

        [HttpGet]
        [Route(Urls.CUSTOMER_ID)]
        public async Task<IActionResult> GetCustomerByIdAsync(int id)
        {
            CustomerDto customer = await customerService.GetCustomerByIdAsync(id);

            return Ok(customer);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [Route(Urls.CUSTOMER_ADD)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerCreateDto dto)
        {
            await customerService.CreateCustomerAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut]
        [Route(Urls.CUSTOMER_EDIT)]
        public async Task<IActionResult> EditCustomerAsync(int id, [FromBody] CustomerEditDto dto)
        {
            if (id != dto.CustomerId)
            {
                return BadRequest("ID mismatch");
            }

            await customerService.EditCustomerAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete]
        [Route(Urls.CUSTOMER_DELETE)]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            await customerService.DeleteCustomerAsync(id);

            return Ok();
        }
    }
}