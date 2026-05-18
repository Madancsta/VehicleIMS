using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
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
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}