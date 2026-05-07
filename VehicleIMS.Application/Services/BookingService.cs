using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<object> CreateBookingAsync(BookingDTO dto)
        {
            var vehicleExists = await _bookingRepository.VehicleExistsAsync(dto.VehicleId);

            if (!vehicleExists)
            {
                throw new Exception("Vehicle not found.");
            }

            var booking = new Booking
            {
                VehicleId = dto.VehicleId,
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                ServiceDescription = dto.ServiceDescription
            };

            await _bookingRepository.AddBookingAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return new
            {
                Message = "Booking created successfully.",
                booking.BookingId
            };
        }

        public async Task<List<object>> GetVehicleBookingsAsync(int vehicleId)
        {
            var bookings = await _bookingRepository.GetBookingsByVehicleIdAsync(vehicleId);

            return bookings.Select(b => new
            {
                b.BookingId,
                b.VehicleId,
                b.BookingDate,
                b.BookingTime,
                b.ServiceDescription,
                b.BookingStatus
            }).Cast<object>().ToList();
        }
    }
}