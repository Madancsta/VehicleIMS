using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IBookingService
    {
        Task<object> CreateBookingAsync(BookingDTO dto);
        Task<List<object>> GetCustomerBookingsAsync(int customerId);
    }
}
