using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "StaffOrAdmin")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    // ── POST api/vehicle ──────────────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> RegisterVehicle([FromBody] RegisterVehicleDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _vehicleService.RegisterVehicleAsync(dto);
        return CreatedAtAction(nameof(GetVehicle), new { id = result.VehicleId }, result);
    }

    // ── GET api/vehicle/{id} ──────────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicle(int id)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        if (vehicle is null)
            return NotFound(new { message = $"Vehicle {id} not found." });

        return Ok(vehicle);
    }

    // ── GET api/vehicle/customer/{customerId} ─────────────────────────────────
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var vehicles = await _vehicleService.GetVehiclesByCustomerAsync(customerId);
        return Ok(vehicles);
    }

    // ── DELETE api/vehicle/{id} ───────────────────────────────────────────────
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var deleted = await _vehicleService.DeleteVehicleAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Vehicle {id} not found." });

        return NoContent();
    }
}
