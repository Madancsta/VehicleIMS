using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Policy = "StaffOrAdmin")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly IEmailService _emailService;

    public SalesController(ISalesService salesService, IEmailService emailService)
    {
        _salesService = salesService;
        _emailService = emailService;
    }

    // ── POST api/sales ────────────────────────────────────────────────────────
    /// <summary>Create a new sale and generate an invoice.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSalesDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _salesService.CreateSaleAsync(dto);
        return CreatedAtAction(nameof(GetSaleById), new { id = result.SalesId }, result);
    }

    // ── GET api/sales ─────────────────────────────────────────────────────────
    /// <summary>List all sales (Admin/Staff only).</summary>
    [HttpGet]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> GetAllSales()
    {
        var sales = await _salesService.GetAllSalesAsync();
        return Ok(sales);
    }

    // ── GET api/sales/{id} ────────────────────────────────────────────────────
    /// <summary>Get a single sale by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSaleById(int id)
    {
        var sale = await _salesService.GetSaleByIdAsync(id);
        if (sale is null)
            return NotFound(new { message = $"Sale {id} not found." });

        return Ok(sale);
    }

    // ── GET api/sales/{id}/invoice ────────────────────────────────────────────
    /// <summary>Get the full invoice summary for a sale.</summary>
    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var invoice = await _salesService.GetInvoiceSummaryAsync(id);
        if (invoice is null)
            return NotFound(new { message = $"Sale {id} not found." });

        return Ok(invoice);
    }

    // ── GET api/sales/customer/{customerId} ───────────────────────────────────
    /// <summary>List all sales for a specific customer.</summary>
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetSalesByCustomer(int customerId)
    {
        var sales = await _salesService.GetSalesByCustomerAsync(customerId);
        return Ok(sales);
    }

    // ── POST api/sales/{id}/send-invoice ─────────────────────────────────────
    [HttpPost("{id:int}/send-invoice")]
    public async Task<IActionResult> SendInvoiceEmail(int id, [FromBody] EmailInvoiceDTO dto)
    {
        if (dto.SalesId != id)
            return BadRequest(new { message = "Route id and body SalesId must match." });

        var invoice = await _salesService.GetInvoiceSummaryAsync(id);
        if (invoice is null)
            return NotFound(new { message = $"Sale {id} not found." });

        var recipient = dto.RecipientEmail ?? invoice.CustomerEmail;
        if (string.IsNullOrWhiteSpace(recipient))
            return BadRequest(new { message = "No email address available for this customer." });

        await _emailService.SendInvoiceEmailAsync(invoice, recipient);

        return Ok(new { message = $"Invoice {invoice.InvoiceNumber} sent to {recipient}." });
    }
}