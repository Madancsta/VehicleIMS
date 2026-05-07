using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> SaleExistsAsync(int salesId);

        Task AddReviewAsync(Review review);

        Task<List<Review>> GetReviewsBySalesIdAsync(int salesId);

        Task SaveChangesAsync();
    }
}