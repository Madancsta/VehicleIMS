using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ICustomerRepository _customerRepository;

    // Handles customer registration, profile, and vehicle API requests
    public CustomerController(
        ICustomerService customerService,
        ICustomerRepository customerRepository)
    {
        _customerService = customerService;
        _customerRepository = customerRepository;
    }

    // Get all customers - only admin or staff can access
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving customers: {ex.Message}" });
        }
    }

    // Register a new customer
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

    // Get logged-in customer's profile
    [Authorize(Roles = "Customer")]
    [HttpGet("{customerId}/profile")]
    public async Task<IActionResult> GetProfile(int customerId)
    {
        if (!await IsOwnCustomer(customerId))
        {
            return Forbid();
        }

        var profile = await _customerService.GetProfileAsync(customerId);

        if (profile == null)
        {
            return NotFound("Customer not found.");
        }

        return Ok(profile);
    }

    // Update logged-in customer's profile
    [Authorize(Roles = "Customer")]
    [HttpPut("{customerId}/profile")]
    public async Task<IActionResult> UpdateProfile(int customerId, CustomerProfileUpdateDTO dto)
    {
        if (!await IsOwnCustomer(customerId))
        {
            return Forbid();
        }

        var updated = await _customerService.UpdateProfileAsync(customerId, dto);

        if (!updated)
        {
            return NotFound("Customer not found.");
        }

        return Ok("Profile updated successfully.");
    }

    // Add a vehicle for the logged-in customer
    [Authorize(Roles = "Customer")]
    [HttpPost("{customerId}/vehicles")]
    public async Task<IActionResult> AddVehicle(int customerId, VehicleCreateUpdateDTO dto)
    {
        if (!await IsOwnCustomer(customerId))
        {
            return Forbid();
        }

        var vehicle = await _customerService.AddVehicleAsync(customerId, dto);

        if (vehicle == null)
        {
            return NotFound("Customer not found.");
        }

        return Ok(vehicle);
    }

    // Update a vehicle owned by the logged-in customer
    [Authorize(Roles = "Customer")]
    [HttpPut("{customerId}/vehicles/{vehicleId}")]
    public async Task<IActionResult> UpdateVehicle(
        int customerId,
        int vehicleId,
        VehicleCreateUpdateDTO dto)
    {
        if (!await IsOwnCustomer(customerId))
        {
            return Forbid();
        }

        var updated = await _customerService.UpdateVehicleAsync(customerId, vehicleId, dto);

        if (!updated)
        {
            return NotFound("Vehicle not found.");
        }

        return Ok("Vehicle updated successfully.");
    }

    // Check if the logged-in user owns the selected customer profile
    private async Task<bool> IsOwnCustomer(int customerId)
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return false;
        }

        var customer = await _customerRepository.GetByUserIdAsync(userId);

        return customer?.CustomerId == customerId;
    }
}