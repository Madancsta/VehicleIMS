using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.DTOs
{
    public class StaffDTO
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public UserStatus Status { get; set; }
    }

    public class CreateStaffDTO
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Staff";
    }

    public class UpdateStaffDTO
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public UserStatus Status { get; set; } = UserStatus.Active;
    }

    public class ChangeUserRoleDTO
    {
        public Guid UserId { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}