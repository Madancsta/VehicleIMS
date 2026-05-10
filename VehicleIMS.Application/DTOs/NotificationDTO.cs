namespace VehicleIMS.Application.DTOs;

public class NotificationDTO
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // "LowStock" or "UnpaidCredit"
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public object? Data { get; set; } // Additional data like PartId, CustomerId, etc.
}

public class LowStockNotificationDTO
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int Threshold { get; set; }
}

public class UnpaidCreditNotificationDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal CreditAmount { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}