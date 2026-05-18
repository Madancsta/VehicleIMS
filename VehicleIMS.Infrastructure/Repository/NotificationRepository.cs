using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    private const int LOW_STOCK_THRESHOLD = 5;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDTO>> GetLowStockNotificationsAsync()
    {
        var lowStockParts = await _context.Parts
            .Where(p => p.StockQuantity <= LOW_STOCK_THRESHOLD && p.StockQuantity > 0)
            .Select(p => new NotificationDTO
            {
                Id = p.PartId,
                Type = "LowStock",
                Title = "Low Stock Alert",
                Message = $"{p.PartName} is running low. Only {p.StockQuantity} left in stock.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Data = new LowStockNotificationDTO
                {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    CurrentStock = p.StockQuantity,
                    Threshold = LOW_STOCK_THRESHOLD
                }
            })
            .ToListAsync();

        var outOfStockParts = await _context.Parts
            .Where(p => p.StockQuantity == 0)
            .Select(p => new NotificationDTO
            {
                Id = p.PartId,
                Type = "LowStock",
                Title = "Out of Stock Alert!",
                Message = $"{p.PartName} is completely out of stock. Please restock immediately.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Data = new LowStockNotificationDTO
                {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    CurrentStock = 0,
                    Threshold = LOW_STOCK_THRESHOLD
                }
            })
            .ToListAsync();

        return lowStockParts.Concat(outOfStockParts).ToList();
    }

    public async Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync()
    {
        var customersWithCredit = await _context.Customers
            .Include(c => c.User)
            .Where(c => c.CreditBalance > 0)
            .Select(c => new NotificationDTO
            {
                Id = c.CustomerId,
                Type = "UnpaidCredit",
                Title = "Unpaid Credit Alert",
                Message = $"{c.FirstName} {c.LastName} has unpaid credit of Rs. {c.CreditBalance:N0}.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Data = new UnpaidCreditNotificationDTO
                {
                    CustomerId = c.CustomerId,
                    CustomerName = $"{c.FirstName} {c.LastName}",
                    CreditAmount = (decimal)c.CreditBalance,
                    Email = c.User.Email ?? "",
                    PhoneNumber = c.User.PhoneNumber ?? ""
                }
            })
            .ToListAsync();

        return customersWithCredit;
    }

    public async Task<object> GetNotificationSummaryAsync()
    {
        var lowStockCount = await _context.Parts
            .CountAsync(p => p.StockQuantity <= LOW_STOCK_THRESHOLD);

        var unpaidCreditCount = await _context.Customers
            .CountAsync(c => c.CreditBalance > 0);

        return new
        {
            LowStockCount = lowStockCount,
            UnpaidCreditCount = unpaidCreditCount,
            TotalUnpaidAmount = await _context.Customers.SumAsync(c => (decimal)c.CreditBalance),
            TotalNotifications = lowStockCount + unpaidCreditCount
        };
    }
}