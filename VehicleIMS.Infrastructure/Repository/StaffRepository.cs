using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Repositories;

namespace VehicleIMS.Infrastructure.Repository;

public class StaffRepository : RepositoryBase<Users>, IStaffRepository
{
    private readonly AppDbContext _context;

    public StaffRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Users>> GetAllStaffAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<Users?> GetStaffByIdAsync(Guid userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Users> CreateStaffAsync(Users user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<Users?> UpdateStaffAsync(Users user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null)
        {
            return null;
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UserName = user.UserName;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Address = user.Address;
        existingUser.Status = user.Status;

        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeactivateStaffAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.Status = Domain.Enums.UserStatus.Inactive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateStaffAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.Status = Domain.Enums.UserStatus.Active;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeRoleAsync(Guid userId, string role)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

       
        return true;
    }
}