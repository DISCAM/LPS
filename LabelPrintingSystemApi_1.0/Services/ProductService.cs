using Data.Dtos.Product;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services
{
    public class ProductService : IProductService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<ProductService> logger;

        public ProductService(DatabaseContext databaseContext, ILogger<ProductService> logger)
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            IQueryable<Product> queryable = databaseContext.Products
                .AsNoTracking()
                .Where(item => item.IsActive);

            return await queryable
                .Select(item => new ProductDto
                {
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    Name = item.Name,
                    Description = item.Description,
                    Ean = item.Ean,
                    Gtin = item.Gtin,
                    IsActive = item.IsActive,
                    CreatedAt = item.CreatedAt,
                    ModifiedAt = item.ModifiedAt
                })
                .ToListAsync();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            Product product = await databaseContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.ProductId == id && item.IsActive)
                ?? throw new NotFoundException("Product not found");

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                Name = product.Name,
                Description = product.Description,
                Ean = product.Ean,
                Gtin = product.Gtin,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                ModifiedAt = product.ModifiedAt
            };
        }

        public async Task CreateProductAsync(ProductCreateDto dto)
        {
            bool productCodeExists = await databaseContext.Products
                .AnyAsync(item => item.ProductCode == dto.ProductCode && item.IsActive);

            if (productCodeExists)
            {
                throw new BadRequestException("Product code already exists");
            }

            Product product = new Product
            {
                ProductCode = dto.ProductCode,
                Name = dto.Name,
                Description = dto.Description,
                Ean = dto.Ean,
                Gtin = dto.Gtin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            databaseContext.Products.Add(product);

            await databaseContext.SaveChangesAsync();

            logger.LogInformation($"Product was created. ProductId: {product.ProductId}");
        }

        public async Task EditProductAsync(ProductEditDto dto)
        {
            Product product = await databaseContext.Products
                .FirstOrDefaultAsync(item => item.ProductId == dto.ProductId && item.IsActive)
                ?? throw new NotFoundException("Product not found");

            bool productCodeExists = await databaseContext.Products
                .AnyAsync(item =>
                    item.ProductCode == dto.ProductCode &&
                    item.ProductId != dto.ProductId &&
                    item.IsActive);

            if (productCodeExists)
            {
                throw new BadRequestException("Product code already exists");
            }

            product.ProductCode = dto.ProductCode;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Ean = dto.Ean;
            product.Gtin = dto.Gtin;
            product.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation($"Product was edited. ProductId: {product.ProductId}");
        }

        public async Task DeleteProductAsync(int id)
        {
            Product product = await databaseContext.Products
                .FirstOrDefaultAsync(item => item.ProductId == id && item.IsActive)
                ?? throw new NotFoundException("Product not found");

            product.IsActive = false;
            product.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation($"Product was deactivated. ProductId: {product.ProductId}");
        }
    }
}