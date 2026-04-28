using Microsoft.AspNetCore.Identity;

namespace VehicleIMS.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}