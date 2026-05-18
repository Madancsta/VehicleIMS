using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Repositories;

namespace VehicleIMS.Infrastructure.Repositories;

public class SalesRepository(AppDbContext context)
    : RepositoryBase<Sales>(context), ISalesRepository
{
    public async Task<Sales?> GetByIdWithDetailsAsync(int salesId)
    {
        return await Context.Sales
            .Include(s => s.Customer).ThenInclude(c => c.User)
            .Include(s => s.Service)
            .Include(s => s.Vehicle)
            .Include(s => s.Booking)
                .ThenInclude(b => b.Vehicle)
            .Include(s => s.SalesItems).ThenInclude(si => si.Part)
            .FirstOrDefaultAsync(s => s.SalesId == salesId);
    }

    public async Task<List<Sales>> GetByCustomerIdAsync(int customerId)
    {
        return await Context.Sales
            .Where(s => s.CustomerId == customerId)
            .Include(s => s.Customer).ThenInclude(c => c.User)
            .Include(s => s.Vehicle)
            .Include(s => s.Service)
            .Include(s => s.SalesItems).ThenInclude(si => si.Part)
            .OrderByDescending(s => s.SalesDate)
            .ToListAsync();
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var today = DateTime.UtcNow;
        var prefix = $"INV-{today:yyyyMMdd}-";

        // Count today's invoices to generate a sequential suffix
        var count = await Context.Sales
            .CountAsync(s => s.InvoiceNumber.StartsWith(prefix));

        return $"{prefix}{(count + 1):D4}";
    }
}