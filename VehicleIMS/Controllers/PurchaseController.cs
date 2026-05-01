using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs.Purchase;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseController(IPurchaseService purchaseService) : ControllerBase
{
    // GET api/purchase
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await purchaseService.GetAllInvoicesAsync();
        return Ok(invoices);
    }

    // GET api/purchase/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await purchaseService.GetInvoiceByIdAsync(id);
        if (invoice is null)
            return NotFound(new { message = $"Invoice with ID {id} not found." });

        return Ok(invoice);
    }

    // POST api/purchase
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await purchaseService.CreateInvoiceAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.PurchaseId }, created);
    }
}