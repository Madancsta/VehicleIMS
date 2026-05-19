using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Controllers;

[Route("api/customer/notifications")]
[ApiController]
[Authorize(Roles = "Customer")]
public class CustomerNotificationController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomerNotificationController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all notifications for the logged-in customer
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Get customer by UserId
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found" });
        }

        var notifications = new List<object>();

        // 1. Get bookings that are completed or pending
        var bookings = await _context.Bookings
            .Include(b => b.Vehicle)
            .Where(b => b.Vehicle.CustomerId == customer.CustomerId)
            .Select(b => new
            {
                Id = b.BookingId,
                Type = "Booking",
                Title = b.BookingStatus == BookingStatus.Completed ? "Booking Completed" : "Booking Update",
                Message = b.BookingStatus == BookingStatus.Completed
                    ? $"Your booking on {b.BookingDate.ToShortDateString()} has been completed."
                    : $"Your booking on {b.BookingDate.ToShortDateString()} is {b.BookingStatus}.",
                CreatedAt = b.BookingDate,
                IsRead = false,
                Status = b.BookingStatus.ToString()
            })
            .ToListAsync();

        notifications.AddRange(bookings);

        // 2. Get part requests
        var requests = await _context.Requests
            .Include(r => r.Booking)
                .ThenInclude(b => b.Vehicle)
            .Where(r => r.Booking.Vehicle.CustomerId == customer.CustomerId)
            .Select(r => new
            {
                Id = r.RequestId,
                Type = "PartRequest",
                Title = r.RequestStatusId == 2 ? "Part Request Approved" : "Part Request Update",
                Message = r.RequestStatusId == 2
                    ? "Your requested parts are now available."
                    : "Your part request has been received and is being processed.",
                CreatedAt = r.RequestedDate,
                IsRead = false,
                Status = r.RequestStatusId == 2 ? "Approved" : "Pending"
            })
            .ToListAsync();

        notifications.AddRange(requests);

        // 3. Get sales/completed services
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .Where(s => s.CustomerId == customer.CustomerId && s.PaymentStatus == PaymentStatus.Completed)
            .Select(s => new
            {
                Id = s.SalesId,
                Type = "Service",
                Title = "Service Completed",
                Message = $"Your service has been completed. Invoice: {s.InvoiceNumber}",
                CreatedAt = s.SalesDate,
                IsRead = false,
                Amount = s.SalesAmount
            })
            .ToListAsync();

        notifications.AddRange(sales);

        // Order by date (newest first)
        var result = notifications
            .OrderByDescending(n => n.GetType().GetProperty("CreatedAt")?.GetValue(n, null))
            .ToList();

        return Ok(new
        {
            total = result.Count,
            notifications = result
        });
    }

    /// <summary>
    /// Get notification summary for customer
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetNotificationSummary()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

        if (customer == null)
        {
            return NotFound();
        }

        // Count pending bookings
        var pendingBookings = await _context.Bookings
            .Include(b => b.Vehicle)
            .CountAsync(b => b.Vehicle.CustomerId == customer.CustomerId && b.BookingStatus == BookingStatus.Pending);

        // Count pending requests
        var pendingRequests = await _context.Requests
            .Include(r => r.Booking)
                .ThenInclude(b => b.Vehicle)
            .CountAsync(r => r.Booking.Vehicle.CustomerId == customer.CustomerId && r.RequestStatusId != 2);

        return Ok(new
        {
            pendingBookings = pendingBookings,
            pendingRequests = pendingRequests,
            totalUnread = pendingBookings + pendingRequests
        });
    }
}