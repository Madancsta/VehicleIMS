using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Repositories;

namespace VehicleIMS.Infrastructure.Repository;

public class ServiceRepository(AppDbContext context)
    : RepositoryBase<Service>(context), IServiceRepository
{
}
