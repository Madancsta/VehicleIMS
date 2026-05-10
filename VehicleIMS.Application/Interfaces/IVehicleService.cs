using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponseDTO> RegisterVehicleAsync(RegisterVehicleDTO dto);
    Task<List<VehicleResponseDTO>> GetVehiclesByCustomerAsync(int customerId);
    Task<VehicleResponseDTO?> GetVehicleByIdAsync(int vehicleId);
    Task<bool> DeleteVehicleAsync(int vehicleId);
}