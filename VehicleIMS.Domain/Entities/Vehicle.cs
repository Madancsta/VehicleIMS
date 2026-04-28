using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleIMS.Domain.Entities;

public class Vehicle
{
    [Key]
    public int VehicleId { get; set; }
    [Required]
    public string VehicleNumber { get; set; }
    [Required]
    public string Model { get; set; }
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Color { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
}
