using Data.Dtos.Product;

namespace LabelPrintingSystemApi_1._0.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        Task<ProductDto> GetProductByIdAsync(int id);

        Task CreateProductAsync(ProductCreateDto dto);

        Task EditProductAsync(ProductEditDto dto);

        Task DeleteProductAsync(int id);
    }
}