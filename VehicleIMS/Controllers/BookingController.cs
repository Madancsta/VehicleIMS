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

        // Handles booking-related API requests
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Get all bookings - only staff or admin can access
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

        // Get bookings made by a specific customer
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

        // Create a new booking
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

        // Get bookings for a specific vehicle
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetVehicleBookings(int vehicleId)
        {
            var bookings = await _bookingService.GetVehicleBookingsAsync(vehicleId);
            return Ok(bookings);
        }

        [HttpGet("{bookingId}/details")]
        public async Task<IActionResult> GetBookingDetails(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingDetailsAsync(bookingId);

                if (booking == null)
                {
                    return NotFound(new { message = $"Booking with ID {bookingId} was not found." });
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving booking details: {ex.Message}" });
            }
        }

    }
}