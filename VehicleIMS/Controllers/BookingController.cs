using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{

    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving bookings: {ex.Message}" });
            }
        }

        // GET: api/bookings/customer/{customerId}
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetBookingsByCustomer(int customerId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByCustomerAsync(customerId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving customer bookings: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingDTO dto)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetVehicleBookings(int vehicleId)
        {
            var bookings = await _bookingService.GetVehicleBookingsAsync(vehicleId);
            return Ok(bookings);
        }
    }
}