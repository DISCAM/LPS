using Data.Dtos.Printer;
using LabelPrintingSystemApi_1._0.Exceptions;
using LabelPrintingSystemApi_1._0.Models;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Services.Konfiguracja
{
    public class PrinterService : IPrinterService
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<PrinterService> logger;

        public PrinterService(
            DatabaseContext databaseContext,
            ILogger<PrinterService> logger
        )
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<PrinterDto>> GetAllPrintersAsync()
        {
            IQueryable<Printer> queryable = databaseContext.Printers
                .AsNoTracking()
                .Where(item => item.IsActive);

            return await queryable
                .Select(item => new PrinterDto
                {
                    PrinterId = item.PrinterId,
                    Name = item.Name,
                    IpAddress = item.IpAddress,
                    Location = item.Location,
                    PrinterModel = item.PrinterModel,
                    IntegrationType = item.IntegrationType,
                    IsActive = item.IsActive,
                    CreatedByUserId = item.CreatedByUserId,
                    ModifiedByUserId = item.ModifiedByUserId,
                    CreatedAt = item.CreatedAt,
                    ModifiedAt = item.ModifiedAt
                })
                .ToListAsync();
        }

        public async Task<PrinterDto> GetPrinterByIdAsync(int id)
        {
            Printer printer = await databaseContext.Printers
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                    item.PrinterId == id &&
                    item.IsActive)
                ?? throw new NotFoundException("Printer not found");

            return new PrinterDto
            {
                PrinterId = printer.PrinterId,
                Name = printer.Name,
                IpAddress = printer.IpAddress,
                Location = printer.Location,
                PrinterModel = printer.PrinterModel,
                IntegrationType = printer.IntegrationType,
                IsActive = printer.IsActive,
                CreatedByUserId = printer.CreatedByUserId,
                ModifiedByUserId = printer.ModifiedByUserId,
                CreatedAt = printer.CreatedAt,
                ModifiedAt = printer.ModifiedAt
            };
        }

        public async Task CreatePrinterAsync(PrinterCreateDto dto)
        {
            bool printerNameExists = await databaseContext.Printers
                .AnyAsync(item => item.Name == dto.Name);

            if (printerNameExists)
            {
                throw new BadRequestException("Printer name already exists");
            }

            bool printerIpAddressExists = await databaseContext.Printers
                .AnyAsync(item => item.IpAddress == dto.IpAddress);

            if (printerIpAddressExists)
            {
                throw new BadRequestException("Printer IP address already exists");
            }

            Printer printer = new Printer
            {
                Name = dto.Name,
                IpAddress = dto.IpAddress,
                Location = dto.Location,
                PrinterModel = dto.PrinterModel,
                IntegrationType = dto.IntegrationType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            databaseContext.Printers.Add(printer);

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                $"Printer was created. PrinterId: {printer.PrinterId}"
            );
        }

        public async Task EditPrinterAsync(PrinterEditDto dto)
        {
            Printer printer = await databaseContext.Printers
                .FirstOrDefaultAsync(item =>
                    item.PrinterId == dto.PrinterId &&
                    item.IsActive)
                ?? throw new NotFoundException("Printer not found");

            bool printerNameExists = await databaseContext.Printers
                .AnyAsync(item =>
                    item.Name == dto.Name &&
                    item.PrinterId != dto.PrinterId);

            if (printerNameExists)
            {
                throw new BadRequestException("Printer name already exists");
            }

            bool printerIpAddressExists = await databaseContext.Printers
                .AnyAsync(item =>
                    item.IpAddress == dto.IpAddress &&
                    item.PrinterId != dto.PrinterId);

            if (printerIpAddressExists)
            {
                throw new BadRequestException("Printer IP address already exists");
            }

            printer.Name = dto.Name;
            printer.IpAddress = dto.IpAddress;
            printer.Location = dto.Location;
            printer.PrinterModel = dto.PrinterModel;
            printer.IntegrationType = dto.IntegrationType;
            printer.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                $"Printer was edited. PrinterId: {printer.PrinterId}"
            );
        }

        public async Task DeletePrinterAsync(int id)
        {
            Printer printer = await databaseContext.Printers
                .FirstOrDefaultAsync(item =>
                    item.PrinterId == id &&
                    item.IsActive)
                ?? throw new NotFoundException("Printer not found");

            printer.IsActive = false;
            printer.ModifiedAt = DateTime.UtcNow;

            await databaseContext.SaveChangesAsync();

            logger.LogInformation(
                $"Printer was deactivated. PrinterId: {printer.PrinterId}"
            );
        }
    }
}