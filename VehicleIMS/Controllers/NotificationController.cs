using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Staff,Customer")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockNotifications()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var notifications = await _notificationService.GetLowStockNotificationsAsync(userId.Value);
        return Ok(notifications);
    }

    [HttpGet("unpaid-credits")]
    public async Task<IActionResult> GetUnpaidCreditNotifications()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var notifications = await _notificationService.GetUnpaidCreditNotificationsAsync(userId.Value);
        return Ok(notifications);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllNotifications()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var notifications = await _notificationService.GetAllNotificationsAsync(userId.Value);
        return Ok(notifications);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetNotificationSummary()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var summary = await _notificationService.GetNotificationSummaryAsync(userId.Value);
        return Ok(summary);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _notificationService.MarkAsReadAsync(userId.Value, id);
        if (!result)
        {
            return NotFound(new { message = "Notification not found" });
        }

        return Ok(new { message = "Marked as read" });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] string? type = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId.Value, type ?? "");
        return Ok(new { message = "All notifications marked as read" });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
