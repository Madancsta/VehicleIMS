using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleIMS.Domain.Entities;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required]
    [ForeignKey(nameof(Users))]
    public Guid UserId { get; set; }
    public Users User { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string LoyaltyPoints { get; set; } = string.Empty;
    public float TotalSpent { get; set; }
    public float CreditBalance { get; set; }
}
