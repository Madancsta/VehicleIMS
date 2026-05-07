using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> CreateBookingAsync(BookingDTO dto)
    {
        var customerExists = await _bookingRepository.CustomerExistsAsync(dto.CustomerId);

        if (!customerExists)
        {
            throw new Exception("Customer not found.");
        }

        var vehicleExists = await _bookingRepository.VehicleBelongsToCustomerAsync(dto.VehicleId, dto.CustomerId);

        if (!vehicleExists)
        {
            throw new Exception("Vehicle not found for this customer.");
        }

        var booking = new Booking
        {
            CustomerId = dto.CustomerId,
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

    public async Task<List<object>> GetCustomerBookingsAsync(int customerId)
    {
        var bookings = await _bookingRepository.GetBookingsByCustomerIdAsync(customerId);

        return bookings.Select(b => new
        {
            b.BookingId,
            b.CustomerId,
            b.VehicleId,
            b.BookingDate,
            b.BookingTime,
            b.ServiceDescription,
            b.BookingStatus
        }).Cast<object>().ToList();
    }
}