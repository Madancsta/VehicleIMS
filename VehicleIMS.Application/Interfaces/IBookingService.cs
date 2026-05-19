using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponseDTO>> GetAllBookingsAsync();
        Task<List<BookingResponseDTO>> GetBookingsByCustomerAsync(int customerId);
        Task<object> CreateBookingAsync(BookingDTO dto);
        Task<object?> GetBookingDetailsAsync(int bookingId);
        Task<List<object>> GetVehicleBookingsAsync(int vehicleId);
    }
}