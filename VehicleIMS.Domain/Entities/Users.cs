using Microsoft.AspNetCore.Identity;
using System;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Domain.Entities;

public class Users : IdentityUser<Guid>
{
    public string Address { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserStatus Status { get; set; } = UserStatus.Active;
}