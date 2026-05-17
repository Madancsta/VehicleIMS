using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> SaleExistsAsync(int salesId);

        Task<bool> SaleBelongsToCustomerAsync(int salesId, int customerId);

        Task<bool> ReviewExistsForSaleAsync(int salesId);

        Task<List<Sales>> GetReviewableSalesByCustomerIdAsync(int customerId);

        Task AddReviewAsync(Review review);

        Task<List<Review>> GetReviewsBySalesIdAsync(int salesId);

        Task SaveChangesAsync();
    }
}
