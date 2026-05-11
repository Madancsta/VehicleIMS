using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repository;

public class CustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerResponseDto>> GetAllCustomers()
    {
        return await _context.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                UserId = c.UserId,
                FullName = c.User.FirstName + " " + c.User.LastName,
                Email = c.User.Email ?? "",
                PhoneNumber = c.User.PhoneNumber ?? "",
                Address = c.User.Address,

                LoyaltyPoints = c.LoyaltyPoints,
                TotalSpent = c.TotalSpent,
                CreditBalance = c.CreditBalance,

                Vehicles = c.Vehicles.Select(v => new VehicleResponseDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    Color = v.Color,
                    Year = v.Year
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<CustomerResponseDto?> GetCustomerById(int id)
    {
        return await _context.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Where(c => c.CustomerId == id)
            .Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                UserId = c.UserId,
                FullName = c.User.FirstName + " " + c.User.LastName,
                Email = c.User.Email ?? "",
                PhoneNumber = c.User.PhoneNumber ?? "",
                Address = c.User.Address,

                LoyaltyPoints = c.LoyaltyPoints,
                TotalSpent = c.TotalSpent,
                CreditBalance = c.CreditBalance,

                Vehicles = c.Vehicles.Select(v => new VehicleResponseDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    Color = v.Color,
                    Year = v.Year
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Customer> CreateCustomer(CustomerCreateDto dto)
    {
        var customer = new Customer
        {
            UserId = dto.UserId,
            LoyaltyPoints = dto.LoyaltyPoints,
            TotalSpent = dto.TotalSpent,
            CreditBalance = dto.CreditBalance
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return customer;
    }
}