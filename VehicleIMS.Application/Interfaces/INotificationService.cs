using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDTO>> GetLowStockNotificationsAsync();
    Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync();
    Task<List<NotificationDTO>> GetAllNotificationsAsync();
    Task<object> GetNotificationSummaryAsync();
    Task<bool> MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(string type = ""); // type can be "LowStock", "UnpaidCredit", or empty for all
}