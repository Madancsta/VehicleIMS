using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IReviewService
    {
        Task<object> CreateReviewAsync(ReviewDTO dto);
        Task<List<object>> GetCustomerReviewsAsync(int customerId);
    }
}
