using System.Data;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Interfaces
{
    public interface IStaffRepository : IRepositoryBase<Users>
    {
        Task<List<Users>> GetAllStaffAsync();

        Task<Users?> GetStaffByIdAsync(int userId);

        Task<Users?> GetUserByEmailAsync(string email);

        Task<Users> CreateStaffAsync(Users user);

        Task<Users?> UpdateStaffAsync(Users user);

        Task<bool> DeactivateStaffAsync(int userId);

        Task<bool> ChangeRoleAsync(int userId, Role role);
    }
}