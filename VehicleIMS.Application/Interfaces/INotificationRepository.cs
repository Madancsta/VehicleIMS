using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<NotificationDTO>> GetLowStockNotificationsAsync();
    Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync();
    Task<object> GetNotificationSummaryAsync();
}