using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public Users User { get; set; } = null!;

    public int LoyaltyPoints { get; set; } = 0;

    public float TotalSpent { get; set; } = 0;

    public float CreditBalance { get; set; } = 0;

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}