using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers;

[Route("api/parts")]
[ApiController]
public class PartsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PartsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetParts()
    {
        var parts = await _context.Parts
            .OrderBy(p => p.PartName)
            .Select(p => new
            {
                p.PartId,
                p.PartName,
                p.PartPrice,
                p.StockQuantity
            })
            .ToListAsync();

        return Ok(parts);
    }
}
