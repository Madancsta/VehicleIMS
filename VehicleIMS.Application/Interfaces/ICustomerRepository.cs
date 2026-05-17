using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface ICustomerRepository
    {

        Task<Customer?> GetByIdWithUserAndVehiclesAsync(int customerId);
        Task<Customer?> GetByIdWithUserAsync(int customerId);
        Task<Customer?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsAsync(int customerId);
        Task AddCustomerAsync(Customer customer);
        Task AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> GetVehicleByIdAsync(int vehicleId);
        Task SaveChangesAsync();

    }
}
