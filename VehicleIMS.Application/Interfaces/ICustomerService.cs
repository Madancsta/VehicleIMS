using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<AuthResponseDTO> RegisterAsync(CustomerRegisterDTO dto);
        Task<CustomerProfileDTO?> GetProfileAsync(int customerId);
        Task<bool> UpdateProfileAsync(int customerId, CustomerProfileUpdateDTO dto);
        Task<object?> AddVehicleAsync(int customerId, VehicleCreateUpdateDTO dto);
        Task<bool> UpdateVehicleAsync(int customerId, int vehicleId, VehicleCreateUpdateDTO dto);

    }
}
