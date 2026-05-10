using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Interfaces
{
    public interface IStaffRepository : IRepositoryBase<Users>
    {
        Task<List<Users>> GetAllStaffAsync();
        Task<Users?> GetStaffByIdAsync(Guid userId);          
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users> CreateStaffAsync(Users user);
        Task<Users?> UpdateStaffAsync(Users user);
        Task<bool> DeactivateStaffAsync(Guid userId);          
        Task<bool> ActivateStaffAsync(Guid userId);            
        Task<bool> ChangeRoleAsync(Guid userId, string role);  
    }
}