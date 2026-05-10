using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public CustomerService(
        ICustomerRepository customerRepository,
        UserManager<Users> userManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _customerRepository = customerRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            throw new Exception("Email already exists.");
        }

        var existingUsername = await _userManager.FindByNameAsync(dto.UserName);

        if (existingUsername != null)
        {
            throw new Exception("Username already exists.");
        }

        var user = new Users
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        try
        {
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = "Customer",
                    Description = "Customer role"
                });
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            var customer = new Customer
            {
                UserId = user.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                LoyaltyPoints = 0,
                TotalSpent = 0,
                CreditBalance = 0
            };

            await _customerRepository.AddCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();

            {
                Success = true,
                Message = "Customer registered successfully.",
            };
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    public async Task<CustomerProfileDTO?> GetProfileAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdWithUserAndVehiclesAsync(customerId);

        if (customer == null)
        {
            return null;
        }

        return new CustomerProfileDTO
        {
            CustomerId = customer.CustomerId,
            UserName = customer.User.UserName ?? "",
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.User.Email ?? "",
            PhoneNumber = customer.User.PhoneNumber ?? "",
            Address = customer.User.Address,
            LoyaltyPoints = customer.LoyaltyPoints,
            TotalSpent = customer.TotalSpent,
            CreditBalance = customer.CreditBalance,
            Vehicles = customer.Vehicles.Select(v => new VehicleDTO
            {
                VehicleId = v.VehicleId,
                VehicleNumber = v.VehicleNumber,
                Brand = v.Brand,
                Model = v.Model,
                Color = v.Color,
                Year = v.Year
            }).ToList()
        };
    }

    public async Task<bool> UpdateProfileAsync(int customerId, CustomerProfileUpdateDTO dto)
    {
        var customer = await _customerRepository.GetByIdWithUserAsync(customerId);

        if (customer == null)
        {
            return false;
        }

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;

        customer.User.UserName = dto.UserName;
        customer.User.FirstName = dto.FirstName;
        customer.User.LastName = dto.LastName;
        customer.User.PhoneNumber = dto.PhoneNumber;
        customer.User.Address = dto.Address;

        await _customerRepository.SaveChangesAsync();

        return true;
    }

    public async Task<object?> AddVehicleAsync(int customerId, VehicleCreateUpdateDTO dto)
    {
        var customerExists = await _customerRepository.ExistsAsync(customerId);

        if (!customerExists)
        {
            return null;
        }

        var vehicle = new Vehicle
        {
            CustomerId = customerId,
            VehicleNumber = dto.VehicleNumber,
            Brand = dto.Brand,
            Model = dto.Model,
            Color = dto.Color,
            Year = dto.Year
        };

        await _customerRepository.AddVehicleAsync(vehicle);
        await _customerRepository.SaveChangesAsync();

        return vehicle;
    }

    {
        var vehicle = await _customerRepository.GetVehicleByIdAsync(vehicleId);

        {
            return false;
        }

        vehicle.VehicleNumber = dto.VehicleNumber;
        vehicle.Brand = dto.Brand;
        vehicle.Model = dto.Model;
        vehicle.Color = dto.Color;
        vehicle.Year = dto.Year;

        await _customerRepository.SaveChangesAsync();

        return true;
    }
}