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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}