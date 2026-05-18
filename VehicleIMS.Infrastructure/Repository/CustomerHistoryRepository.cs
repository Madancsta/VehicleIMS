using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Repositories;

namespace VehicleIMS.Infrastructure.Repository
{
    public class CustomerHistoryRepository : RepositoryBase<Customer>, ICustomerHistoryRepository
    {
        private readonly AppDbContext _context;

        public CustomerHistoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CustomerHistoryDTO?> GetCustomerHistoryAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return null;
            }

            var customerVehicles = await _context.Vehicles
                .Where(v => v.CustomerId == customerId)
                .ToListAsync();

            var customerVehicleIds = customerVehicles
                .Select(v => v.VehicleId)
                .ToList();
            
            var bookings = new List<CustomerBookingHistoryDTO>();

            var sales = new List<CustomerSalesHistoryDTO>();

            var reviews = new List<CustomerReviewHistoryDTO>();

            var requests = await _context.Requests
                .Where(r => r.BookingId != null)
                .Select(r => new CustomerRequestHistoryDTO
                {
                    RequestId = r.RequestId,
                    RequestedDate = r.RequestedDate,
                    RequestStatus = r.RequestStatusId.ToString(),
                    BookingId = r.BookingId
                })
                .ToListAsync();

            return new CustomerHistoryDTO
            {
                CustomerId = customer.CustomerId,
                UserId = customer.User.Id,
                FirstName = customer.User.FirstName,
                LastName = customer.User.LastName,
                Email = customer.User.Email ?? string.Empty,
                PhoneNumber = customer.User.PhoneNumber ?? string.Empty,
                Address = customer.User.Address,

                Bookings = bookings,
                Sales = sales,
                Reviews = reviews,
                Requests = requests
            };
        }
    }
}