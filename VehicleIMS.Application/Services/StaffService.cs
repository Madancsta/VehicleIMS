using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public StaffService(
            UserManager<Users> userManager,
            RoleManager<Role> roleManager)          
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<StaffDTO>> GetAllStaffAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var staffList = new List<StaffDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin") || roles.Contains("Staff"))
                {
                    staffList.Add(new StaffDTO
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = user.PhoneNumber ?? string.Empty,
                        Address = user.Address,
                        Role = roles.FirstOrDefault() ?? string.Empty,
                        Status = user.Status
                    });
                }
            }

            return staffList;
        }

        public async Task<StaffDTO?> GetStaffByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new StaffDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                Role = roles.FirstOrDefault() ?? string.Empty,
                Status = user.Status
            };
        }

        public async Task<StaffDTO?> CreateStaffAsync(CreateStaffDTO createStaffDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(createStaffDTO.Email);

            if (existingUser != null)
            {
                return null;
            }

            var roleExists = await _roleManager.RoleExistsAsync(createStaffDTO.Role);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new Role { Name = createStaffDTO.Role });
            }

            var user = new Users
            {
                Id = Guid.NewGuid(),
                FirstName = createStaffDTO.FirstName,
                LastName = createStaffDTO.LastName,
                Email = createStaffDTO.Email,
                UserName = createStaffDTO.Email,
                PhoneNumber = createStaffDTO.PhoneNumber,
                Address = createStaffDTO.Address,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, createStaffDTO.Password);

            if (!createResult.Succeeded)
            {
                return null;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, createStaffDTO.Role);

            if (!roleResult.Succeeded)
            {
                return null;
            }

            return new StaffDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                Role = createStaffDTO.Role,
                Status = user.Status
            };
        }

        public async Task<StaffDTO?> UpdateStaffAsync(Guid userId, UpdateStaffDTO updateStaffDTO)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return null;
            }

            user.FirstName = updateStaffDTO.FirstName;
            user.LastName = updateStaffDTO.LastName;
            user.Email = updateStaffDTO.Email;
            user.UserName = updateStaffDTO.Email;
            user.PhoneNumber = updateStaffDTO.PhoneNumber;
            user.Address = updateStaffDTO.Address;
            user.Status = updateStaffDTO.Status;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new StaffDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                Role = roles.FirstOrDefault() ?? string.Empty,
                Status = user.Status
            };
        }

        public async Task<bool> DeactivateStaffAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            user.Status = UserStatus.Inactive;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> ActivateStaffAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            user.Status = UserStatus.Active;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> ChangeRoleAsync(ChangeUserRoleDTO changeUserRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(changeUserRoleDTO.UserId.ToString());

            if (user == null)
            {
                return false;
            }

            var roleExists = await _roleManager.RoleExistsAsync(changeUserRoleDTO.Role);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new Role { Name = changeUserRoleDTO.Role });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    return false;
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, changeUserRoleDTO.Role);

            return addResult.Succeeded;
        }
    }
}