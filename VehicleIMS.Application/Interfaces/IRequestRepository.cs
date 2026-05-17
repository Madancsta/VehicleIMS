using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces
{

    public interface IRequestRepository
    {
        Task<bool> BookingExistsAsync(int bookingId);
        Task<bool> PartExistsAsync(int partId);
        Task AddRequestAsync(Request request);
        Task AddRequestPartAsync(RequestPart requestPart);
        Task<List<Request>> GetRequestsByBookingIdAsync(int bookingId);
        Task<List<Request>> GetRequestsByCustomerIdAsync(int customerId);
        Task SaveChangesAsync();
    }
}
