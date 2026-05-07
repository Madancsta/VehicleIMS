using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> VehicleBelongsToCustomerAsync(int vehicleId, int customerId)
        {
            return await _context.Vehicles
                .AnyAsync(v => v.VehicleId == vehicleId && v.CustomerId == customerId);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task<List<Booking>> GetBookingsByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
