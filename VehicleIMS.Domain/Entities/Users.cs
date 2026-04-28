using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using VehicleIMS.Domain.Enums;  

namespace VehicleIMS.Domain.Entities;
public class Users
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string Email { get; set; }
    [Required]
    public long PhoneNumber { get; set; }
    [Required]
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public UserStatus Status { get; set; } = UserStatus.Active;
}
