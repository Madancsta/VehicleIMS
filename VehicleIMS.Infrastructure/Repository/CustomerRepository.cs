using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .ToListAsync();
        }

        public async Task<List<Customer>> SearchCustomersAsync(string query)
        {
            query = query.ToLower();

            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Where(c =>
                    c.CustomerId.ToString().Contains(query) ||
                    c.FirstName.ToLower().Contains(query) ||
                    c.LastName.ToLower().Contains(query) ||
                    (c.FirstName + " " + c.LastName).ToLower().Contains(query) ||
                    (c.User.PhoneNumber != null && c.User.PhoneNumber.Contains(query)) ||
                    c.Vehicles.Any(v => v.VehicleNumber.ToLower().Contains(query))
                )
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
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

        public async Task<Customer?> GetByIdWithUserAndVehiclesAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Customer?> GetByIdWithUserAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Customer?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> ExistsAsync(int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int vehicleId)
        {
            return await _context.Vehicles.FindAsync(vehicleId);
        }

        public async Task<List<Customer>> GetHighSpendersAsync()
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Where(c => c.TotalSpent >= 5000)
                .OrderByDescending(c => c.TotalSpent)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetPendingCreditsAsync()
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Where(c => c.CreditBalance > 0)
                .OrderByDescending(c => c.CreditBalance)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetRegularCustomersAsync()
        {
            // Get customer IDs with more than 3 completed sales
            var customerIds = await _context.Sales
                .GroupBy(s => s.CustomerId)
                .Where(g => g.Count() > 3)
                .Select(g => g.Key)
                .ToListAsync();

            // Fetch those customers
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Where(c => customerIds.Contains(c.CustomerId))
                .OrderByDescending(c => _context.Sales.Count(s => s.CustomerId == c.CustomerId))
                .ToListAsync();
        }
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}