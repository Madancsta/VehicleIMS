using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requestRepository;

    public RequestService(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<object> CreatePartRequestAsync(PartRequestDTO dto)
    {
        var bookingExists = await _requestRepository.BookingExistsAsync(dto.BookingId);

        if (!bookingExists)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        var bookingIsCompleted = await _requestRepository.BookingIsCompletedAsync(dto.BookingId);

        if (bookingIsCompleted)
        {
            throw new ArgumentException("Cannot submit part request for a completed booking.");
        }

        var partExists = await _requestRepository.PartExistsAsync(dto.PartId);

        if (!partExists)
        {
            throw new KeyNotFoundException("Part not found.");
        }

        var request = new Request
        {
            BookingId = dto.BookingId,
            RequestStatusId = 1,
            RequestedDate = DateTime.UtcNow
        };

        await _requestRepository.AddRequestAsync(request);
        await _requestRepository.SaveChangesAsync();

        var requestPart = new RequestPart
        {
            RequestId = request.RequestId,
            PartId = dto.PartId,
            RequestQuantity = dto.RequestQuantity,
            RequestDescription = dto.RequestDescription
        };

        await _requestRepository.AddRequestPartAsync(requestPart);
        await _requestRepository.SaveChangesAsync();

        return new
        {
            Message = "Part request submitted successfully.",
            request.RequestId
        };
    }

    public async Task<List<object>> GetRequestsByBookingAsync(int bookingId)
    {
        var requests = await _requestRepository.GetRequestsByBookingIdAsync(bookingId);

        return requests.Select(r => new
        {
            r.RequestId,
            r.BookingId,
            r.RequestStatusId,
            r.RequestedDate,
            Parts = r.RequestParts.Select(rp => new
            {
                rp.PartId,
                rp.Part.PartName,
                rp.RequestQuantity,
                rp.RequestDescription
            })
        }).Cast<object>().ToList();
    }

    public async Task<List<object>> GetRequestsByCustomerAsync(int customerId)
    {
        var requests = await _requestRepository.GetRequestsByCustomerIdAsync(customerId);

        return requests.Select(r => new
        {
            r.RequestId,
            r.BookingId,
            r.RequestStatusId,
            r.RequestedDate,
            VehicleId = r.Booking.VehicleId,
            VehicleName = $"{r.Booking.Vehicle.Brand} {r.Booking.Vehicle.Model}",
            Parts = r.RequestParts.Select(rp => new
            {
                rp.PartId,
                rp.Part.PartName,
                rp.RequestQuantity,
                rp.RequestDescription
            })
        }).Cast<object>().ToList();
    }

    public async Task<object> ApprovePartRequestAsync(int requestId)
    {
        var request = await _requestRepository.GetRequestByIdAsync(requestId);

        if (request == null)
        {
            throw new KeyNotFoundException("Part request not found.");
        }

        request.RequestStatusId = 2; // Approved

        await _requestRepository.SaveChangesAsync();

        return new
        {
            Message = "Part request approved and booking completed successfully.",
            request.RequestId,
            request.BookingId,
            request.RequestStatusId,
            BookingStatus = request.Booking.BookingStatus.ToString()
        };
    }

    public async Task<object> RejectPartRequestAsync(int requestId)
    {
        var request = await _requestRepository.GetRequestByIdAsync(requestId);

        if (request == null)
        {
            throw new KeyNotFoundException("Part request not found.");
        }

        if (request.RequestStatusId == 3)
        {
            return new
            {
                Message = "Part request is already rejected.",
                request.RequestId,
                request.BookingId,
                request.RequestStatusId
            };
        }

        request.RequestStatusId = 3; // Rejected

        await _requestRepository.SaveChangesAsync();

        return new
        {
            Message = "Part request rejected successfully.",
            request.RequestId,
            request.BookingId,
            request.RequestStatusId
        };
    }

    public async Task<List<object>> GetAllRequestsAsync()
    {
        var requests = await _requestRepository.GetAllRequestsAsync();

        return requests.Select(r => new
        {
            r.RequestId,
            r.BookingId,
            r.RequestStatusId,
            r.RequestedDate,
            BookingStatus = r.Booking.BookingStatus.ToString(),
            VehicleName = $"{r.Booking.Vehicle.Brand} {r.Booking.Vehicle.Model}",
            Parts = r.RequestParts.Select(rp => new
            {
                rp.PartId,
                rp.Part.PartName,
                rp.RequestQuantity,
                rp.RequestDescription
            })
        }).Cast<object>().ToList();
    }

}
