// IServiceService.cs
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Interfaces
{
    public interface IServiceService
    {
        Task<List<ServiceResponseDTO>> GetAllServicesAsync();
    }
}