using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRepositoryBase<Booking> _bookingRepo;

        public BookingService(
            IRepositoryBase<Booking> bookingRepo,
            IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
            _bookingRepo = bookingRepo;
        }

        // GET: Get all bookings
        public async Task<List<BookingResponseDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepo
                .FindAll() 
                .Include(b => b.Vehicle)  
                    .ThenInclude(v => v.Customer)
                        .ThenInclude(c => c.User)
                .OrderByDescending(b => b.BookingDate)
                .ThenByDescending(b => b.BookingTime)
                .ToListAsync();

            return bookings.Select(MapToResponse).ToList();
        }

        // Get all bookings for a customer
        public async Task<List<BookingResponseDTO>> GetBookingsByCustomerAsync(int customerId)
        {
            var bookings = await _bookingRepository.GetBookingsByCustomerAsync(customerId);
            return bookings.Select(MapToResponse).ToList();
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
                BookingDate = DateTime.SpecifyKind(dto.BookingDate, DateTimeKind.Utc),
                BookingTime = dto.BookingTime,
                ServiceType = dto.ServiceType,
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
                b.ServiceType,
                b.ServiceDescription,
                b.BookingStatus
            }).Cast<object>().ToList();
        }

        private static BookingResponseDTO MapToResponse(Booking b) => new()
        {
            BookingId = b.BookingId,
            VehicleId = b.VehicleId,
            VehicleInfo = b.Vehicle != null
                ? $"{b.Vehicle.Brand} {b.Vehicle.Model} ({b.Vehicle.Year}) - {b.Vehicle.VehicleNumber}"
                : null,
            CustomerName = b.Vehicle?.Customer?.User != null
                ? $"{b.Vehicle.Customer.User.FirstName} {b.Vehicle.Customer.User.LastName}"
                : null,
            CustomerId = b.Vehicle?.CustomerId,
            ServiceType = b.ServiceType,
            ServiceDescription = b.ServiceDescription,
            BookingDate = b.BookingDate,
            BookingTime = b.BookingTime,
            BookingStatus = b.BookingStatus.ToString()
        };

    }
}