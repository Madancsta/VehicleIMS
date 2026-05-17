using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Application.Services
{
    public class CustomerHistoryService : ICustomerHistoryService
    {
        private readonly ICustomerHistoryRepository _customerHistoryRepository;

        public CustomerHistoryService(ICustomerHistoryRepository customerHistoryRepository)
        {
            _customerHistoryRepository = customerHistoryRepository;
        }

        public async Task<CustomerHistoryDTO?> GetCustomerHistoryAsync(int customerId)
        {
            return await _customerHistoryRepository.GetCustomerHistoryAsync(customerId);
        }
    }
}