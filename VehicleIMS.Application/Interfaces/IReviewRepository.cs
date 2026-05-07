using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> SaleExistsAsync(int salesId);
        Task<bool> CustomerExistsAsync(int customerId);
        Task AddReviewAsync(Review review);
        Task<List<Review>> GetReviewsByCustomerIdAsync(int customerId);
        Task SaveChangesAsync();
    }
}