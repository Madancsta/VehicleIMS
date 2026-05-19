using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers;

[Route("api/customer/notifications")]
[ApiController]
[Authorize(Roles = "Customer")]
public class CustomerNotificationController : ControllerBase
{
    private readonly AppDbContext _context;

    private const int BOOKING_ID_OFFSET = 200000;
    private const int PART_REQUEST_ID_OFFSET = 300000;
    private const int SERVICE_ID_OFFSET = 400000;

    public CustomerNotificationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var customer = await GetCurrentCustomerAsync();

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found" });
        }

        var notifications = await BuildCustomerNotificationsAsync(customer);

        return Ok(new
        {
            total = notifications.Count,
            notifications
        });
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetNotificationSummary()
    {
        var customer = await GetCurrentCustomerAsync();

        if (customer == null)
        {
            return NotFound();
        }

        var notifications = await BuildCustomerNotificationsAsync(customer);
        var unread = notifications.Where(n => !n.IsRead).ToList();

        return Ok(new
        {
            pendingBookings = unread.Count(n => n.Type == "Booking" && n.Status == "Pending"),
            pendingRequests = unread.Count(n => n.Type == "PartRequest" && n.Status == "Pending"),
            totalUnread = unread.Count,
            totalNotifications = notifications.Count
        });
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var customer = await GetCurrentCustomerAsync();

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found" });
        }

        var notification = (await BuildCustomerNotificationsAsync(customer))
            .FirstOrDefault(n => n.Id == id);

        if (notification == null)
        {
            return NotFound(new { message = "Notification not found" });
        }

        await MarkNotificationKeyAsReadAsync(customer.UserId, notification.NotificationKey);
        return Ok(new { message = "Marked as read" });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var customer = await GetCurrentCustomerAsync();

        if (customer == null)
        {
            return NotFound(new { message = "Customer profile not found" });
        }

        var notifications = await BuildCustomerNotificationsAsync(customer);
        await MarkNotificationKeysAsReadAsync(
            customer.UserId,
            notifications.Where(n => !n.IsRead).Select(n => n.NotificationKey));

        return Ok(new { message = "All notifications marked as read" });
    }

    private async Task<Customer?> GetCurrentCustomerAsync()
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return null;
        }

        return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    private async Task<List<CustomerNotificationItem>> BuildCustomerNotificationsAsync(Customer customer)
    {
        var readKeys = await _context.NotificationReadStates
            .Where(n => n.UserId == customer.UserId)
            .Select(n => n.NotificationKey)
            .ToListAsync();

        var readKeySet = readKeys.ToHashSet();
        var notifications = new List<CustomerNotificationItem>();

        var bookings = await _context.Bookings
            .Include(b => b.Vehicle)
            .Where(b => b.Vehicle.CustomerId == customer.CustomerId)
            .Select(b => new
            {
                b.BookingId,
                b.BookingDate,
                b.BookingStatus
            })
            .ToListAsync();

        notifications.AddRange(bookings.Select(b =>
        {
            var status = b.BookingStatus.ToString();
            var key = BuildKey("Booking", b.BookingId);

            return new CustomerNotificationItem
            {
                Id = BOOKING_ID_OFFSET + b.BookingId,
                NotificationKey = key,
                Type = "Booking",
                Title = b.BookingStatus == BookingStatus.Completed
                    ? "Booking Completed"
                    : "Booking Update",
                Message = b.BookingStatus == BookingStatus.Completed
                    ? $"Your booking on {b.BookingDate.ToShortDateString()} has been completed."
                    : $"Your booking on {b.BookingDate.ToShortDateString()} is {status}.",
                CreatedAt = b.BookingDate,
                IsRead = readKeySet.Contains(key),
                Status = status
            };
        }));

        var requests = await _context.Requests
            .Include(r => r.Booking)
                .ThenInclude(b => b.Vehicle)
            .Where(r => r.Booking.Vehicle.CustomerId == customer.CustomerId)
            .Select(r => new
            {
                r.RequestId,
                r.RequestedDate,
                r.RequestStatusId
            })
            .ToListAsync();

        notifications.AddRange(requests.Select(r =>
        {
            var status = r.RequestStatusId == 2 ? "Approved" : "Pending";
            var key = BuildKey("PartRequest", r.RequestId);

            return new CustomerNotificationItem
            {
                Id = PART_REQUEST_ID_OFFSET + r.RequestId,
                NotificationKey = key,
                Type = "PartRequest",
                Title = r.RequestStatusId == 2 ? "Part Request Approved" : "Part Request Update",
                Message = r.RequestStatusId == 2
                    ? "Your requested parts are now available."
                    : "Your part request has been received and is being processed.",
                CreatedAt = r.RequestedDate,
                IsRead = readKeySet.Contains(key),
                Status = status
            };
        }));

        var sales = await _context.Sales
            .Where(s => s.CustomerId == customer.CustomerId && s.PaymentStatus == PaymentStatus.Completed)
            .Select(s => new
            {
                s.SalesId,
                s.InvoiceNumber,
                s.SalesDate,
                s.SalesAmount
            })
            .ToListAsync();

        notifications.AddRange(sales.Select(s =>
        {
            var key = BuildKey("Service", s.SalesId);

            return new CustomerNotificationItem
            {
                Id = SERVICE_ID_OFFSET + s.SalesId,
                NotificationKey = key,
                Type = "Service",
                Title = "Service Completed",
                Message = $"Your service has been completed. Invoice: {s.InvoiceNumber}",
                CreatedAt = s.SalesDate,
                IsRead = readKeySet.Contains(key),
                Amount = s.SalesAmount
            };
        }));

        return notifications
            .OrderBy(n => n.IsRead)
            .ThenByDescending(n => n.CreatedAt)
            .ToList();
    }

    private async Task MarkNotificationKeyAsReadAsync(Guid userId, string notificationKey)
    {
        if (await _context.NotificationReadStates.AnyAsync(
                n => n.UserId == userId && n.NotificationKey == notificationKey))
        {
            return;
        }

        await _context.NotificationReadStates.AddAsync(new NotificationReadState
        {
            UserId = userId,
            NotificationKey = notificationKey,
            ReadAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    private async Task MarkNotificationKeysAsReadAsync(Guid userId, IEnumerable<string> notificationKeys)
    {
        var keys = notificationKeys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Distinct()
            .ToList();

        if (!keys.Any())
        {
            return;
        }

        var existingKeys = await _context.NotificationReadStates
            .Where(n => n.UserId == userId && keys.Contains(n.NotificationKey))
            .Select(n => n.NotificationKey)
            .ToListAsync();

        var existing = existingKeys.ToHashSet();
        var newReadStates = keys
            .Where(key => !existing.Contains(key))
            .Select(key => new NotificationReadState
            {
                UserId = userId,
                NotificationKey = key,
                ReadAt = DateTime.UtcNow
            })
            .ToList();

        if (!newReadStates.Any())
        {
            return;
        }

        await _context.NotificationReadStates.AddRangeAsync(newReadStates);
        await _context.SaveChangesAsync();
    }

    private static string BuildKey(string type, int sourceId)
    {
        return $"{type}:{sourceId}";
    }

    private sealed class CustomerNotificationItem
    {
        public int Id { get; set; }
        public string NotificationKey { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
    }
}
