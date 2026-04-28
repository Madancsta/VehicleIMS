using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs.Part;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController(IPartService partService) : ControllerBase
{
    // GET api/parts
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var parts = await partService.GetAllPartsAsync();
        return Ok(parts);
    }

    // GET api/parts/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var part = await partService.GetPartByIdAsync(id);
        if (part is null)
            return NotFound(new { message = $"Part with ID {id} not found." });

        return Ok(part);
    }

    // POST api/parts
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await partService.CreatePartAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.PartId }, created);
    }

    // PUT api/parts/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await partService.UpdatePartAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"Part with ID {id} not found." });

        return Ok(updated);
    }

    // DELETE api/parts/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await partService.DeletePartAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Part with ID {id} not found." });

        return NoContent();
    }
}