using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<object> CreateReviewAsync(ReviewDTO dto, int customerId)
        {
            var belongsToCustomer = await _reviewRepository
                .SaleBelongsToCustomerAsync(dto.SalesId, customerId);

            if (!belongsToCustomer)
            {
                throw new Exception("Sales record not found for this customer.");
            }

            var reviewExists = await _reviewRepository.ReviewExistsForSaleAsync(dto.SalesId);

            if (reviewExists)
            {
                throw new Exception("Review already exists for this sale.");
            }

            var review = new Review
            {
                SalesId = dto.SalesId,
                Rating = dto.Rating,
                ReviewComment = dto.ReviewComment,
                ReviewDate = DateTime.UtcNow
            };

            await _reviewRepository.AddReviewAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return new
            {
                Message = "Review submitted successfully.",
                review.ReviewId
            };
        }

        public async Task<List<ReviewableSaleDTO>> GetReviewableSalesByCustomerIdAsync(int customerId)
        {
            var sales = await _reviewRepository.GetReviewableSalesByCustomerIdAsync(customerId);

            return sales.Select(s => new ReviewableSaleDTO
            {
                SalesId = s.SalesId,
                BookingId = s.BookingId ?? 0,
                ServiceType = s.Booking.ServiceType,
                VehicleNumber = s.Booking.Vehicle.VehicleNumber,
                SalesDate = s.SalesDate,
                SalesAmount = s.SalesAmount
            }).ToList();
        }


        public async Task<List<object>> GetReviewsBySalesIdAsync(int salesId)
        {
            var reviews = await _reviewRepository.GetReviewsBySalesIdAsync(salesId);

            return reviews.Select(r => new
            {
                r.ReviewId,
                r.SalesId,
                r.Rating,
                r.ReviewComment,
                r.ReviewDate
            }).Cast<object>().ToList();
        }
    }
}