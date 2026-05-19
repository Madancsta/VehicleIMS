using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[Route("api/requests")]
[ApiController]
public class RequestController : ControllerBase
{
    private readonly IRequestService _requestService;

    // Handles unavailable part request API actions
    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    // Create a new unavailable part request
    [HttpPost]
    public async Task<IActionResult> CreatePartRequest(PartRequestDTO dto)
    {
        try
        {
            var result = await _requestService.CreatePartRequestAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Get part requests related to a specific booking
    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetRequestsByBooking(int bookingId)
    {
        var requests = await _requestService.GetRequestsByBookingAsync(bookingId);
        return Ok(requests);
    }

    // Get all part requests made by a specific customer
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetRequestsByCustomer(int customerId)
    {
        var requests = await _requestService.GetRequestsByCustomerAsync(customerId);
        return Ok(requests);
    }

    [HttpGet]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _requestService.GetAllRequestsAsync();
        return Ok(requests);
    }

    [HttpPut("{requestId}/approve")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> ApprovePartRequest(int requestId)
    {
        try
        {
            var result = await _requestService.ApprovePartRequestAsync(requestId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{requestId}/reject")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> RejectPartRequest(int requestId)
    {
        try
        {
            var result = await _requestService.RejectPartRequestAsync(requestId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}