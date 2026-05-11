namespace VehicleIMS.Application.DTOs;

public class CustomerResponseDto
{
    public int CustomerId { get; set; }

    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public int LoyaltyPoints { get; set; }
    public float TotalSpent { get; set; }
    public float CreditBalance { get; set; }

    public List<VehicleResponseDto> Vehicles { get; set; } = new();
}

public class VehicleResponseDto
{
    public int VehicleId { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Year { get; set; }
}