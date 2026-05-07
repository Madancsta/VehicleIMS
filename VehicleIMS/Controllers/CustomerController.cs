using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public CustomerController(
            AppDbContext context,
            UserManager<Users> userManager,
            RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CustomerRegisterDTO dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            var existingUsername = await _userManager.FindByNameAsync(dto.UserName);

            if (existingUsername != null)
            {
                return BadRequest("Username already exists.");
            }

            var user = new Users
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

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

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var vehicle = new Vehicle
            {
                CustomerId = customer.CustomerId,
                VehicleNumber = dto.VehicleNumber,
                Brand = dto.Brand,
                Model = dto.Model,
                Color = dto.Color,
                Year = dto.Year
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Customer registered successfully.",
                customer.CustomerId,
                vehicle.VehicleId
            });
        }

        [HttpGet("{customerId}/profile")]
        public async Task<IActionResult> GetProfile(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var profile = new CustomerProfileDTO
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

            return Ok(profile);
        }

        [HttpPut("{customerId}/profile")]
        public async Task<IActionResult> UpdateProfile(int customerId, CustomerProfileUpdateDTO dto)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.User.UserName = dto.UserName;
            customer.User.PhoneNumber = dto.PhoneNumber;
            customer.User.Address = dto.Address;

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully.");
        }

        [HttpPost("{customerId}/vehicles")]
        public async Task<IActionResult> AddVehicle(int customerId, VehicleCreateUpdateDTO dto)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == customerId);

            if (!customerExists)
            {
                return NotFound("Customer not found.");
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

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return Ok(vehicle);
        }

        [HttpPut("vehicles/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(int vehicleId, VehicleCreateUpdateDTO dto)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);

            if (vehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Brand = dto.Brand;
            vehicle.Model = dto.Model;
            vehicle.Color = dto.Color;
            vehicle.Year = dto.Year;

            await _context.SaveChangesAsync();

            return Ok("Vehicle updated successfully.");
        }
    }
}