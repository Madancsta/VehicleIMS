using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> VehicleExistsAsync(int vehicleId);

        Task AddBookingAsync(Booking booking);

        Task<List<Booking>> GetBookingsByVehicleIdAsync(int vehicleId);
        Task<List<Booking>> GetBookingsByCustomerAsync(int customerId);
        Task<Booking?> GetByIdWithDetailsAsync(int bookingId);
        Task<Object?> GetBookingDetailsAsync(int bookingId);
        Task SaveChangesAsync();
    }
}