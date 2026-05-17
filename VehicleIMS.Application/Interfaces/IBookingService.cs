using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IBookingService
    {
        Task<object> CreateBookingAsync(BookingDTO dto);

        Task<List<object>> GetVehicleBookingsAsync(int vehicleId);
    }
}