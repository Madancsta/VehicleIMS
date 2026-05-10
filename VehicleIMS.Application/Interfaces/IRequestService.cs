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
    }
}
