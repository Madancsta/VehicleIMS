using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var notifications = await _notificationService.GetLowStockNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("unpaid-credits")]
    public async Task<IActionResult> GetUnpaidCreditNotifications()
    {
        var notifications = await _notificationService.GetUnpaidCreditNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _notificationService.GetAllNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetNotificationSummary()
    {
        var summary = await _notificationService.GetNotificationSummaryAsync();
        return Ok(summary);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Notification not found" });
        }
        return Ok(new { message = "Marked as read" });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] string? type = null)
    {
        await _notificationService.MarkAllAsReadAsync(type ?? "");
        return Ok(new { message = "All notifications marked as read" });
    }
}