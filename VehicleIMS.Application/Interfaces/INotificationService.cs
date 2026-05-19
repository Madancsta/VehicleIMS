using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDTO>> GetLowStockNotificationsAsync(Guid userId);
    Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync(Guid userId);
    Task<List<NotificationDTO>> GetAllNotificationsAsync(Guid userId);
    Task<object> GetNotificationSummaryAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid userId, int notificationId);
    Task MarkAllAsReadAsync(Guid userId, string type = ""); // type can be "LowStock", "OutOfStock", "UnpaidCredit", or empty for all
}
