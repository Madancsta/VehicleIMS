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

        public async Task<object> CreateReviewAsync(ReviewDTO dto)
        {
            var saleExists = await _reviewRepository.SaleExistsAsync(dto.SalesId);

            if (!saleExists)
            {
                throw new Exception("Sales record not found.");
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