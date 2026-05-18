using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace VehicleIMS.Controllers;

[Route("api/customer/purchase-history")]
[ApiController]
[Authorize(Roles = "Customer")]
public class PurchaseHistoryController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly AppDbContext _context;

    public PurchaseHistoryController(ISalesService salesService, AppDbContext context)
    {
        _salesService = salesService;
        _context = context;
    }

    /// <summary>
    /// Get current customer's purchase history
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyPurchaseHistory()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not found" });
        }

        // Get customer by UserId
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found." });
        }

        var sales = await _salesService.GetSalesByCustomerAsync(customer.CustomerId);

        return Ok(new
        {
            customerId = customer.CustomerId,
            totalSpent = sales.Sum(s => s.SalesAmount),
            totalOrders = sales.Count,
            orders = sales.Select(s => new
            {
                s.SalesId,
                s.InvoiceNumber,
                s.SalesDate,
                s.SalesAmount,
                s.PaymentStatus,
                s.PaymentMethod,
                s.Items,
                s.ServiceType,
                s.VehicleType
            }).OrderByDescending(s => s.SalesDate)
        });
    }

    /// <summary>
    /// Get specific order details by Sales ID
    /// </summary>
    [HttpGet("orders/{salesId}")]
    public async Task<IActionResult> GetOrderDetails(int salesId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not found" });
        }

        // Get customer by UserId
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found." });
        }

        var sale = await _salesService.GetSaleByIdAsync(salesId);

        if (sale == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        // Verify this order belongs to the customer
        if (sale.CustomerId != customer.CustomerId)
        {
            return Forbid("You can only view your own orders.");
        }

        var invoice = await _salesService.GetInvoiceSummaryAsync(salesId);

        return Ok(new
        {
            sale.SalesId,
            sale.InvoiceNumber,
            sale.SalesDate,
            sale.SalesAmount,
            sale.PaymentStatus,
            sale.PaymentMethod,
            sale.PartsTotal,
            sale.ServiceCharge,
            sale.Discount,
            sale.Items,
            serviceInfo = invoice?.ServiceInfo,
            customerInfo = new
            {
                sale.CustomerName,
                sale.CustomerEmail
            }
        });
    }

    /// <summary>
    /// Get purchase summary (total spent, order count, etc.)
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetPurchaseSummary()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not found" });
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found." });
        }

        var sales = await _salesService.GetSalesByCustomerAsync(customer.CustomerId);

        return Ok(new
        {
            customerId = customer.CustomerId,
            customerName = $"{customer.FirstName} {customer.LastName}",
            totalSpent = sales.Sum(s => s.SalesAmount),
            totalOrders = sales.Count,
            averageOrderValue = sales.Any() ? sales.Average(s => s.SalesAmount) : 0,
            lastOrderDate = sales.Any() ? sales.Max(s => s.SalesDate) : (DateTime?)null,
            loyaltyPoints = customer.LoyaltyPoints,
            creditBalance = customer.CreditBalance
        });
    }
}