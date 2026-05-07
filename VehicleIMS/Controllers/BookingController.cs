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