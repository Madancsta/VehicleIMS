namespace VehicleIMS.Application.DTOs;

public class CustomerCreateDto
{
    public Guid UserId { get; set; }
    public int LoyaltyPoints { get; set; }
    public float TotalSpent { get; set; }
    public float CreditBalance { get; set; }
}