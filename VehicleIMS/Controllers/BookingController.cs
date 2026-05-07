using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingDTO dto)
        {
            var vehicleExists = await _context.Vehicles.AnyAsync(v => v.VehicleId == dto.VehicleId);

            if (!vehicleExists)
            {
                return NotFound("Vehicle not found.");
            }

            var booking = new Booking
            {
                VehicleId = dto.VehicleId,
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                ServiceDescription = dto.ServiceDescription,
                BookingStatus = BookingStatus.Pending
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerBookings(int customerId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Vehicle)
                .Where(b => b.Vehicle.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return Ok(bookings);
        }
    }
}