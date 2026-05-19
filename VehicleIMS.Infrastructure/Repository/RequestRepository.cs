using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;

        public RequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BookingExistsAsync(int bookingId)
        {
            return await _context.Bookings.AnyAsync(b => b.BookingId == bookingId);
        }

        public async Task<bool> PartExistsAsync(int partId)
        {
            return await _context.Parts.AnyAsync(p => p.PartId == partId);
        }

        public async Task AddRequestAsync(Request request)
        {
            await _context.Requests.AddAsync(request);
        }

        public async Task AddRequestPartAsync(RequestPart requestPart)
        {
            await _context.RequestParts.AddAsync(requestPart);
        }

        public async Task<List<Request>> GetRequestsByBookingIdAsync(int bookingId)
        {
            return await _context.Requests
                .Where(r => r.BookingId == bookingId)
                .Include(r => r.RequestParts)
                .ThenInclude(rp => rp.Part)
                .ToListAsync();
        }

        public async Task<List<Request>> GetRequestsByCustomerIdAsync(int customerId)
        {
            return await _context.Requests
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Vehicle)
                .Include(r => r.RequestParts)
                    .ThenInclude(rp => rp.Part)
                .Where(r => r.Booking.Vehicle.CustomerId == customerId)
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();
        }

        public async Task<Request?> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Requests
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}