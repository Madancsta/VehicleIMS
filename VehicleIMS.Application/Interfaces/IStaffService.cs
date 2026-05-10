using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IStaffService
    {
        Task<List<StaffDTO>> GetAllStaffAsync();

        Task<StaffDTO?> GetStaffByIdAsync(Guid userId);

        Task<StaffDTO?> CreateStaffAsync(CreateStaffDTO createStaffDTO);

        Task<StaffDTO?> UpdateStaffAsync(Guid userId, UpdateStaffDTO updateStaffDTO);

        Task<bool> DeactivateStaffAsync(Guid userId);

        Task<bool> ActivateStaffAsync(Guid userId);

        Task<bool> ChangeRoleAsync(ChangeUserRoleDTO changeUserRoleDTO);
    }
}