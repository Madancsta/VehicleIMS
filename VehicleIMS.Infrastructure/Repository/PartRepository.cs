using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories;

public class PartRepository(AppDbContext context)
    : RepositoryBase<Part>(context), IPartRepository
{
    public async Task<IEnumerable<Part>> GetAllWithCategoryAsync()
    {
        return await Context.Parts
            .Include(p => p.PartCategory)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Part?> GetByIdWithCategoryAsync(int id)
    {
        return await Context.Parts
            .Include(p => p.PartCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PartId == id);
    }
}