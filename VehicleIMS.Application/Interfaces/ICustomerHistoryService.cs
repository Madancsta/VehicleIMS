using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface ICustomerHistoryService
    {
        Task<CustomerHistoryDTO?> GetCustomerHistoryAsync(int customerId);
    }
}