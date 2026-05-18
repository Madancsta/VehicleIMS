using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories;

public class PurchaseRepository(AppDbContext context)
    : RepositoryBase<Purchase>(context), IPurchaseRepository
{
}