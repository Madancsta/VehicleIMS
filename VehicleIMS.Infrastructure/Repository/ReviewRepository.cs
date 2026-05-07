using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaleExistsAsync(int salesId)
        {
            return await _context.Sales
                .AnyAsync(s => s.SalesId == salesId);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task<List<Review>> GetReviewsBySalesIdAsync(int salesId)
        {
            return await _context.Reviews
                .Where(r => r.SalesId == salesId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}