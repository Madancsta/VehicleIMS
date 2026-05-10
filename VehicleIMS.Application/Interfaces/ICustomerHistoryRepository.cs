using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface ICustomerHistoryRepository : IRepositoryBase<Customer>
    {
        Task<CustomerHistoryDTO?> GetCustomerHistoryAsync(int customerId);
    }
}