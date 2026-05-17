using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[Route("api/requests")]
[ApiController]
public class RequestController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

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

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetRequestsByBooking(int bookingId)
    {
        var requests = await _requestService.GetRequestsByBookingAsync(bookingId);
        return Ok(requests);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetRequestsByCustomer(int customerId)
    {
        var requests = await _requestService.GetRequestsByCustomerAsync(customerId);
        return Ok(requests);
    }

}