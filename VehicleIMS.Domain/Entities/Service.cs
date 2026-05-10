using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Domain.Entities;

public class Service
{
    [Key]
    public int ServiceId { get; set; }
    [Required]
    public VehicleType VehicleType { get; set; }
    [Required]
    public string ServiceType { get; set; }
    [Required]
    public float ServiceCharge { get; set; }
}
