using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IReviewService
    {
        Task<object> CreateReviewAsync(ReviewDTO dto, int customerId);

        Task<List<ReviewableSaleDTO>> GetReviewableSalesByCustomerIdAsync(int customerId);

        Task<List<object>> GetReviewsBySalesIdAsync(int salesId);
    }
}
