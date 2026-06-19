using Data;
using Data.Dtos.Product;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers
{
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        [Route(Urls.PRODUCTS)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            IEnumerable<ProductDto> products = await productService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route(Urls.PRODUCT_ID)]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            ProductDto product = await productService.GetProductByIdAsync(id);

            return Ok(product);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [Route(Urls.PRODUCT_ADD)]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductCreateDto dto)
        {
            await productService.CreateProductAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut]
        [Route(Urls.PRODUCT_EDIT)]
        public async Task<IActionResult> EditProductAsync(int id, [FromBody] ProductEditDto dto)
        {
            if (id != dto.ProductId)
            {
                return BadRequest("ID mismatch");
            }

            await productService.EditProductAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete]
        [Route(Urls.PRODUCT_DELETE)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            await productService.DeleteProductAsync(id);

            return Ok();
        }
    }
}