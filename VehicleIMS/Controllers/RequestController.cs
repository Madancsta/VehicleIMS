using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers
{
    [ApiController]
    [Route("api/requests")]
    public class RequestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePartRequest(PartRequestDTO dto)
        {
            var bookingExists = await _context.Bookings.AnyAsync(b => b.BookingId == dto.BookingId);

            if (!bookingExists)
            {
                return NotFound("Booking not found.");
            }

            var partExists = await _context.Parts.AnyAsync(p => p.PartId == dto.PartId);

            if (!partExists)
            {
                return NotFound("Part not found.");
            }

            var request = new Request
            {
                BookingId = dto.BookingId,
                RequestStatusId = 1,
                RequestedDate = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestPart = new RequestPart
            {
                RequestId = request.RequestId,
                PartId = dto.PartId,
                RequestQuantity = dto.RequestQuantity,
                RequestDescription = dto.RequestDescription
            };

            _context.RequestParts.Add(requestPart);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Part request submitted successfully.",
                request.RequestId
            });
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetRequestsByBooking(int bookingId)
        {
            var requests = await _context.Requests
                .Include(r => r.RequestParts)
                .ThenInclude(rp => rp.Part)
                .Where(r => r.BookingId == bookingId)
                .ToListAsync();

            return Ok(requests);
        }
    }
}