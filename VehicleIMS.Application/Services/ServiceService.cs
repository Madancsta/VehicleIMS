// ServiceService.cs
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IRepositoryBase<Service> _serviceRepository;

        public ServiceService(IRepositoryBase<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }


        // GET: Get all services (using RepositoryBase pattern)
        public async Task<List<ServiceResponseDTO>> GetAllServicesAsync()
        {
            var services = await _serviceRepository
                .FindAll()
                .OrderBy(s => s.VehicleType)
                .ThenBy(s => s.ServiceType)
                .ToListAsync();

            return services.Select(MapToResponse).ToList();
        }

        private static ServiceResponseDTO MapToResponse(Service s) => new()
        {
            ServiceId = s.ServiceId,
            VehicleType = s.VehicleType,
            VehicleTypeName = s.VehicleType.ToString(),
            ServiceType = s.ServiceType,
            ServiceCharge = s.ServiceCharge,
            FormattedServiceCharge = $"Rs. {s.ServiceCharge:N2}"
        };
    }
}