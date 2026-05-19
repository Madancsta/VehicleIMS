using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> VehicleExistsAsync(int vehicleId)
        {
            return await _context.Vehicles
                .AnyAsync(v => v.VehicleId == vehicleId);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task<List<Booking>> GetBookingsByVehicleIdAsync(int vehicleId)
        {
            return await _context.Bookings
                .Where(b => b.VehicleId == vehicleId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdWithDetailsAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        // Get all bookings for a specific customer
        public async Task<List<Booking>> GetBookingsByCustomerAsync(int customerId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Customer)
                        .ThenInclude(c => c.User)
                .Where(b => b.Vehicle.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ThenByDescending(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<object?> GetBookingDetailsAsync(int bookingId)
        {
            // Query 1: Get booking with vehicle and customer
            var booking = await _context.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => new
                {
                    b.BookingId,
                    b.VehicleId,
                    b.ServiceType,
                    b.ServiceDescription,
                    b.BookingDate,
                    b.BookingTime,
                    b.BookingStatus,
                    Vehicle = b.Vehicle,
                    Customer = b.Vehicle.Customer,
                    User = b.Vehicle.Customer.User
                })
                .FirstOrDefaultAsync();

            if (booking == null)
                return null;

            // Query 2: Get parts for this booking
            var parts = await _context.Requests
                .Where(r => r.BookingId == bookingId)
                .SelectMany(r => r.RequestParts)
                .Select(rp => new
                {
                    rp.PartId,
                    rp.Part.PartName,
                    rp.RequestQuantity,
                    rp.Part.PartPrice
                })
                .ToListAsync();

            // Return combined result
            return new
            {
                booking.BookingId,
                booking.VehicleId,
                VehicleInfo = $"{booking.Vehicle.Brand} {booking.Vehicle.Model} ({booking.Vehicle.Year}) - {booking.Vehicle.VehicleNumber}",
                booking.Customer?.CustomerId,
                CustomerName = booking.User != null ? $"{booking.User.FirstName} {booking.User.LastName}" : null,
                booking.User?.Email,
                booking.User?.PhoneNumber,
                booking.ServiceType,
                booking.ServiceDescription,
                booking.BookingDate,
                booking.BookingTime,
                BookingStatus = booking.BookingStatus.ToString(),
                Parts = parts.Select(p => new
                {
                    p.PartId,
                    p.PartName,
                    Quantity = p.RequestQuantity,
                    UnitPrice = p.PartPrice,
                    TotalPrice = p.PartPrice * p.RequestQuantity
                }).ToList()
            };
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}