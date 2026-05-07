using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewDTO dto)
        {
            var sales = await _context.Sales
                .Include(s => s.Booking)
                .ThenInclude(b => b.Vehicle)
                .FirstOrDefaultAsync(s => s.SalesId == dto.SalesId);

            if (sales == null)
            {
                return NotFound("Sales record not found.");
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.SalesId == dto.SalesId);

            if (existingReview != null)
            {
                return BadRequest("Review already exists for this sale.");
            }

            var review = new Review
            {
                SalesId = dto.SalesId,
                Rating = dto.Rating,
                ReviewComment = dto.ReviewComment,
                ReviewDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }

        [HttpGet("sales/{salesId}")]
        public async Task<IActionResult> GetReviewBySales(int salesId)
        {
            var review = await _context.Reviews
                .Include(r => r.Sales)
                .ThenInclude(s => s.Booking)
                .ThenInclude(b => b.Vehicle)
                .FirstOrDefaultAsync(r => r.SalesId == salesId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            return Ok(review);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReviewsByCustomer(int customerId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Sales)
                .ThenInclude(s => s.Booking)
                .ThenInclude(b => b.Vehicle)
                .Where(r =>
                    r.Sales != null &&
                    r.Sales.Booking != null &&
                    r.Sales.Booking.Vehicle != null &&
                    r.Sales.Booking.Vehicle.CustomerId == customerId
                )
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return Ok(reviews);
        }
    }
}