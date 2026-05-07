using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> CustomerExistsAsync(int customerId);
        Task<bool> VehicleBelongsToCustomerAsync(int vehicleId, int customerId);
        Task AddBookingAsync(Booking booking);
        Task<List<Booking>> GetBookingsByCustomerIdAsync(int customerId);
        Task SaveChangesAsync();
    }
}
