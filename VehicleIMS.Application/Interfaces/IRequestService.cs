using System;
using System.Collections.Generic;
using System.Text;
using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IRequestService
    {
        Task<object> CreatePartRequestAsync(PartRequestDTO dto);
        Task<List<object>> GetRequestsByBookingAsync(int bookingId);
        Task<List<object>> GetRequestsByCustomerAsync(int customerId);
        Task<object> ApprovePartRequestAsync(int requestId);
        Task<object> RejectPartRequestAsync(int requestId);
        Task<List<object>> GetAllRequestsAsync();
    }
}
