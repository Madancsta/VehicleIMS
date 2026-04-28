using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleIMS.Domain.Enums;
using System.Text;

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
    public BookingStatus BookingStatus { get; set; }

    [Required]
    [ForeignKey(nameof(Vehicle))]
    public int VehicleId { get; set; }

}
