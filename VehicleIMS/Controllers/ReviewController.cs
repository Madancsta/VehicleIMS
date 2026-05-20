using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ICustomerRepository _customerRepository;

        // Handles customer review API actions
        public ReviewController(
            IReviewService reviewService,
            ICustomerRepository customerRepository)
        {
            _reviewService = reviewService;
            _customerRepository = customerRepository;
        }

        // Get the logged-in customer's ID from the JWT token
        private async Task<int?> GetCurrentCustomerIdAsync()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return null;
            }

            var customer = await _customerRepository.GetByUserIdAsync(userId);
            return customer?.CustomerId;
        }

        // Get sales that the logged-in customer can review
        [Authorize(Roles = "Customer")]
        [HttpGet("my-reviewable-sales")]
        public async Task<IActionResult> GetMyReviewableSales()
        {
            var customerId = await GetCurrentCustomerIdAsync();

            if (customerId == null)
            {
                return Forbid();
            }

            var sales = await _reviewService.GetReviewableSalesByCustomerIdAsync(customerId.Value);
            return Ok(sales);
        }

        // Create a review for a completed sale
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewDTO dto)
        {
            var customerId = await GetCurrentCustomerIdAsync();

            if (customerId == null)
            {
                return Forbid();
            }

            var result = await _reviewService.CreateReviewAsync(dto, customerId.Value);
            return Ok(result);
        }

        // Get reviews for a specific sale
        [HttpGet("sales/{salesId}")]
        public async Task<IActionResult> GetReviewsBySales(int salesId)
        {
            var reviews = await _reviewService.GetReviewsBySalesIdAsync(salesId);
            return Ok(reviews);
        }
    }
}
