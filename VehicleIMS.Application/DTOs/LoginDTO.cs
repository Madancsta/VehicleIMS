using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Application.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}