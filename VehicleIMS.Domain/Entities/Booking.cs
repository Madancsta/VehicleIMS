using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Domain.Entities;

public class Booking
{
    [Key]
    public int BookingId { get; set; }

    [Required]
    public DateTime BookingDate { get; set; }

    [Required]
    public TimeSpan BookingTime { get; set; }

    [Required]
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;

    [Required]
    [StringLength(50)]
    public string ServiceType { get; set; } = string.Empty;

    [Required]
    public string ServiceDescription { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(Vehicle))]
    public int VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
}