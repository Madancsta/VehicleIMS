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

    public async Task<List<NotificationDTO>> GetLowStockNotificationsAsync()
    {
        return await _notificationRepository.GetLowStockNotificationsAsync();
    }

    public async Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync()
    {
        return await _notificationRepository.GetUnpaidCreditNotificationsAsync();
    }

    public async Task<List<NotificationDTO>> GetAllNotificationsAsync()
    {
        var lowStock = await GetLowStockNotificationsAsync();
        var unpaidCredits = await GetUnpaidCreditNotificationsAsync();

        var all = lowStock.Concat(unpaidCredits)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        for (int i = 0; i < all.Count; i++)
        {
            all[i].Id = i + 1;
        }

        return all;
    }

    public async Task<object> GetNotificationSummaryAsync()
    {
        return await _notificationRepository.GetNotificationSummaryAsync();
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        return await Task.FromResult(true);
    }

    public async Task MarkAllAsReadAsync(string type = "")
    {
        await Task.CompletedTask;
    }
}