using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> VehicleExistsAsync(int vehicleId);

        Task AddBookingAsync(Booking booking);

        Task<List<Booking>> GetBookingsByVehicleIdAsync(int vehicleId);

        Task SaveChangesAsync();
    }
}