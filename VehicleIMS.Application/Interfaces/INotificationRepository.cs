using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<NotificationDTO>> GetLowStockNotificationsAsync(Guid userId);
    Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid userId, string notificationKey);
    Task MarkManyAsReadAsync(Guid userId, IEnumerable<string> notificationKeys);
}
