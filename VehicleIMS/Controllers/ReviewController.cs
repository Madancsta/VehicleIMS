using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{

    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewDTO dto)
        {
            try
            {
                var result = await _reviewService.CreateReviewAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("sales/{salesId}")]
        public async Task<IActionResult> GetReviewsBySales(int salesId)
        {
            var reviews = await _reviewService.GetReviewsBySalesIdAsync(salesId);
            return Ok(reviews);
        }
    }
}