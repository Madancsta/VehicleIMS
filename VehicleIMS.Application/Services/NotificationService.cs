using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDTO>> GetLowStockNotificationsAsync(Guid userId)
    {
        return await _notificationRepository.GetLowStockNotificationsAsync(userId);
    }

    public async Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync(Guid userId)
    {
        return await _notificationRepository.GetUnpaidCreditNotificationsAsync(userId);
    }

    public async Task<List<NotificationDTO>> GetAllNotificationsAsync(Guid userId)
    {
        var lowStock = await GetLowStockNotificationsAsync(userId);
        var unpaidCredits = await GetUnpaidCreditNotificationsAsync(userId);

        return lowStock.Concat(unpaidCredits)
            .OrderBy(n => n.IsRead)
            .ThenByDescending(n => n.CreatedAt)
            .ToList();
    }

    public async Task<object> GetNotificationSummaryAsync(Guid userId)
    {
        var all = await GetAllNotificationsAsync(userId);
        var unread = all.Where(n => !n.IsRead).ToList();

        return new
        {
            LowStockCount = unread.Count(n => n.Type == "LowStock" || n.Type == "OutOfStock"),
            UnpaidCreditCount = unread.Count(n => n.Type == "UnpaidCredit"),
            TotalUnread = unread.Count,
            TotalNotifications = all.Count,
            TotalUnpaidAmount = all
                .Where(n => n.Type == "UnpaidCredit")
                .Select(n => n.Data as UnpaidCreditNotificationDTO)
                .Where(n => n != null)
                .Sum(n => n!.CreditAmount)
        };
    }

    public async Task<bool> MarkAsReadAsync(Guid userId, int notificationId)
    {
        var notification = (await GetAllNotificationsAsync(userId))
            .FirstOrDefault(n => n.Id == notificationId);

        if (notification == null)
        {
            return false;
        }

        await _notificationRepository.MarkAsReadAsync(userId, notification.NotificationKey);
        return true;
    }

    public async Task MarkAllAsReadAsync(Guid userId, string type = "")
    {
        var notifications = await GetAllNotificationsAsync(userId);

        if (!string.IsNullOrWhiteSpace(type))
        {
            notifications = notifications
                .Where(n => string.Equals(n.Type, type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        await _notificationRepository.MarkManyAsReadAsync(
            userId,
            notifications.Where(n => !n.IsRead).Select(n => n.NotificationKey));
    }
}
