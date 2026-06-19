using Data.Dtos.Customer;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<CustomerService> logger;

        public CustomerService(DatabaseContext databaseContext, ILogger<CustomerService> logger)
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            IQueryable<Customer> queryable = databaseContext.Customers
                .AsNoTracking()
                .Where(item => item.IsActive);

            return await queryable
                .Select(item => new CustomerDto
                {
                    CustomerId = item.CustomerId,
                    CustomerCode = item.CustomerCode,
                    Name = item.Name,
                    TaxNumber = item.TaxNumber,
                    Email = item.Email,
                    Phone = item.Phone,
                    Street = item.Street,
                    PostalCode = item.PostalCode,
                    City = item.City,
                    Country = item.Country,
                    IsActive = item.IsActive,
                    CreatedByUserId = item.CreatedByUserId,
                    ModifiedByUserId = item.ModifiedByUserId,
                    CreatedAt = item.CreatedAt,
                    ModifiedAt = item.ModifiedAt
                })
                .ToListAsync();
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            Customer customer = await databaseContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.CustomerId == id && item.IsActive)
                ?? throw new NotFoundException("Customer not found");

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customer.CustomerCode,
                Name = customer.Name,
                TaxNumber = customer.TaxNumber,
                Email = customer.Email,
                Phone = customer.Phone,
                Street = customer.Street,
                PostalCode = customer.PostalCode,
                City = customer.City,
                Country = customer.Country,
                IsActive = customer.IsActive,
                CreatedByUserId = customer.CreatedByUserId,
                ModifiedByUserId = customer.ModifiedByUserId,
                CreatedAt = customer.CreatedAt,
                ModifiedAt = customer.ModifiedAt
            };
        }

        public async Task CreateCustomerAsync(CustomerCreateDto dto)
        {
            bool customerCodeExists = await databaseContext.Customers
                .AnyAsync(item => item.CustomerCode == dto.CustomerCode && item.IsActive);

            if (customerCodeExists)
            {
                throw new BadRequestException("Customer code already exists");
            }

            Customer customer = new Customer
            {
                CustomerCode = dto.CustomerCode,
                Name = dto.Name,
                TaxNumber = dto.TaxNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                Street = dto.Street,
                PostalCode = dto.PostalCode,
                City = dto.City,
                Country = dto.Country,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            databaseContext.Customers.Add(customer);

            await databaseContext.SaveChangesAsync();

            logger.LogInformation($"Customer was created. CustomerId: {customer.CustomerId}");
        }

        public async Task EditCustomerAsync(CustomerEditDto dto)
        {
            Customer customer = await databaseContext.Customers
                .FirstOrDefaultAsync(item => item.CustomerId == dto.CustomerId && item.IsActive)
                ?? throw new NotFoundException("Customer not found");

            bool customerCodeExists = await databaseContext.Customers
                .AnyAsync(item =>
                    item.CustomerCode == dto.CustomerCode &&
                    item.CustomerId != dto.CustomerId &&
                    item.IsActive);

            if (customerCodeExists)
            {
                throw new BadRequestException("Customer code already exists");
            }

            customer.CustomerCode = dto.CustomerCode;
            customer.Name = dto.Name;
            customer.TaxNumber = dto.TaxNumber;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Street = dto.Street;
            customer.PostalCode = dto.PostalCode;
            customer.City = dto.City;
            customer.Country = dto.Country;
            customer.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation($"Customer was edited. CustomerId: {customer.CustomerId}");
        }

        public async Task DeleteCustomerAsync(int id)
        {
            Customer customer = await databaseContext.Customers
                .FirstOrDefaultAsync(item => item.CustomerId == id && item.IsActive)
                ?? throw new NotFoundException("Customer not found");

            customer.IsActive = false;
            customer.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation($"Customer was deactivated. CustomerId: {customer.CustomerId}");
        }
    }
}