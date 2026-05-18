using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace VehicleIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartCategoryController(AppDbContext context) : ControllerBase
{
    // Get all part categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await context.PartCategories.ToListAsync();
        return Ok(categories);
    }
}