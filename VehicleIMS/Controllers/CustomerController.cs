using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CustomerRegisterDTO dto)
    {
        try
        {
            var result = await _customerService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{customerId}/profile")]
    public async Task<IActionResult> GetProfile(int customerId)
    {
        var profile = await _customerService.GetProfileAsync(customerId);

        if (profile == null)
        {
            return NotFound("Customer not found.");
        }

        return Ok(profile);
    }

    [HttpPut("{customerId}/profile")]
    public async Task<IActionResult> UpdateProfile(int customerId, CustomerProfileUpdateDTO dto)
    {
        var updated = await _customerService.UpdateProfileAsync(customerId, dto);

        if (!updated)
        {
            return NotFound("Customer not found.");
        }

        return Ok("Profile updated successfully.");
    }

    [HttpPost("{customerId}/vehicles")]
    public async Task<IActionResult> AddVehicle(int customerId, VehicleCreateUpdateDTO dto)
    {
        var vehicle = await _customerService.AddVehicleAsync(customerId, dto);

        if (vehicle == null)
        {
            return NotFound("Customer not found.");
        }

        return Ok(vehicle);
    }

    [HttpPut("vehicles/{vehicleId}")]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, VehicleCreateUpdateDTO dto)
    {
        var updated = await _customerService.UpdateVehicleAsync(vehicleId, dto);

        if (!updated)
        {
            return NotFound("Vehicle not found.");
        }

        return Ok("Vehicle updated successfully.");
    }
}