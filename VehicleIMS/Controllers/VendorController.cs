using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs.Vendor;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorController(IVendorService vendorService) : ControllerBase
{
    // GET api/vendor
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vendors = await vendorService.GetAllVendorsAsync();
        return Ok(vendors);
    }

    // GET api/vendor/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vendor = await vendorService.GetVendorByIdAsync(id);
        if (vendor is null)
            return NotFound(new { message = $"Vendor with ID {id} not found." });

        return Ok(vendor);
    }

    // POST api/vendor
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await vendorService.CreateVendorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.VendorId }, created);
    }

    // PUT api/vendor/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVendorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await vendorService.UpdateVendorAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"Vendor with ID {id} not found." });

        return Ok(updated);
    }

    // DELETE api/vendor/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await vendorService.DeleteVendorAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Vendor with ID {id} not found." });

        return NoContent();
    }
}