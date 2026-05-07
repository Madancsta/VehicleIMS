using System;
using System.Collections.Generic;
using System.Text;
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
            return await _context.Sales.AnyAsync(s => s.SalesId == salesId);
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task<List<Review>> GetReviewsByCustomerIdAsync(int customerId)
        {
            return await _context.Reviews
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}