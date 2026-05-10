using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IReviewService
    {
        Task<object> CreateReviewAsync(ReviewDTO dto);

        Task<List<object>> GetReviewsBySalesIdAsync(int salesId);
    }
}