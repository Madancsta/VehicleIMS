using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    private const int LOW_STOCK_THRESHOLD = 5;
    private const int UNPAID_CREDIT_ID_OFFSET = 100000;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDTO>> GetLowStockNotificationsAsync(Guid userId)
    {
        var readKeys = await GetReadKeysAsync(userId);

        var lowStockParts = await _context.Parts
            .Where(p => p.StockQuantity <= LOW_STOCK_THRESHOLD && p.StockQuantity > 0)
            .Select(p => new
            {
                p.PartId,
                p.PartName,
                p.StockQuantity
            })
            .ToListAsync();

        var outOfStockParts = await _context.Parts
            .Where(p => p.StockQuantity == 0)
            .Select(p => new
            {
                p.PartId,
                p.PartName,
                p.StockQuantity
            })
            .ToListAsync();

        var notifications = lowStockParts.Select(p =>
        {
            var key = BuildKey("LowStock", p.PartId);

            return new NotificationDTO
            {
                Id = p.PartId,
                NotificationKey = key,
                Type = "LowStock",
                Title = "Low Stock Alert",
                Message = $"{p.PartName} is running low. Only {p.StockQuantity} left in stock.",
                CreatedAt = DateTime.UtcNow,
                IsRead = readKeys.Contains(key),
                Data = new LowStockNotificationDTO
                {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    CurrentStock = p.StockQuantity,
                    Threshold = LOW_STOCK_THRESHOLD
                }
            };
        }).ToList();

        notifications.AddRange(outOfStockParts.Select(p =>
        {
            var key = BuildKey("OutOfStock", p.PartId);

            return new NotificationDTO
            {
                Id = p.PartId,
                NotificationKey = key,
                Type = "OutOfStock",
                Title = "Out of Stock Alert!",
                Message = $"{p.PartName} is completely out of stock. Please restock immediately.",
                CreatedAt = DateTime.UtcNow,
                IsRead = readKeys.Contains(key),
                Data = new LowStockNotificationDTO
                {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    CurrentStock = 0,
                    Threshold = LOW_STOCK_THRESHOLD
                }
            };
        }));

        return notifications;
    }

    public async Task<List<NotificationDTO>> GetUnpaidCreditNotificationsAsync(Guid userId)
    {
        var readKeys = await GetReadKeysAsync(userId);

        var customersWithCredit = await _context.Customers
            .Include(c => c.User)
            .Where(c => c.CreditBalance > 0)
            .Select(c => new
            {
                c.CustomerId,
                c.FirstName,
                c.LastName,
                c.CreditBalance,
                Email = c.User.Email ?? "",
                PhoneNumber = c.User.PhoneNumber ?? ""
            })
            .ToListAsync();

        return customersWithCredit.Select(c =>
        {
            var key = BuildKey("UnpaidCredit", c.CustomerId);

            return new NotificationDTO
            {
                Id = UNPAID_CREDIT_ID_OFFSET + c.CustomerId,
                NotificationKey = key,
                Type = "UnpaidCredit",
                Title = "Unpaid Credit Alert",
                Message = $"{c.FirstName} {c.LastName} has unpaid credit of Rs. {c.CreditBalance:N0}.",
                CreatedAt = DateTime.UtcNow,
                IsRead = readKeys.Contains(key),
                Data = new UnpaidCreditNotificationDTO
                {
                    CustomerId = c.CustomerId,
                    CustomerName = $"{c.FirstName} {c.LastName}",
                    CreditAmount = (decimal)(c.CreditBalance ?? 0),
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber
                }
            };
        }).ToList();
    }

    public async Task MarkAsReadAsync(Guid userId, string notificationKey)
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

    public async Task MarkManyAsReadAsync(Guid userId, IEnumerable<string> notificationKeys)
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

    private async Task<HashSet<string>> GetReadKeysAsync(Guid userId)
    {
        var readKeys = await _context.NotificationReadStates
            .Where(n => n.UserId == userId)
            .Select(n => n.NotificationKey)
            .ToListAsync();

        return readKeys.ToHashSet();
    }

    private static string BuildKey(string type, int sourceId)
    {
        return $"{type}:{sourceId}";
    }
}
