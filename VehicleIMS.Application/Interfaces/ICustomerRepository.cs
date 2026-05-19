using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllCustomers();
        Task<List<Customer>> SearchCustomersAsync(string query);
        Task<Customer?> GetCustomerById(int id);
        Task<Customer> CreateCustomer(CustomerCreateDto dto);
        Task<Customer?> GetByIdWithUserAndVehiclesAsync(int customerId);
        Task<Customer?> GetByIdWithUserAsync(int customerId);
        Task<Customer?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsAsync(int customerId);
        Task AddCustomerAsync(Customer customer);
        Task AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> GetVehicleByIdAsync(int vehicleId);
        Task<List<Customer>> GetHighSpendersAsync();
        Task<List<Customer>> GetPendingCreditsAsync();
        Task<List<Customer>> GetRegularCustomersAsync();
        Task UpdateAsync(Customer customer);
        Task SaveChangesAsync();
    }
}