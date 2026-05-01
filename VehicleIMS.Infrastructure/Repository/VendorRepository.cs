using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories;

public class VendorRepository(AppDbContext context)
    : RepositoryBase<Vendor>(context), IVendorRepository
{
}