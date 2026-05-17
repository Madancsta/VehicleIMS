using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IRepositoryBase<Vehicle> _vehicleRepo;
    private readonly IRepositoryBase<Customer> _customerRepo;

    public VehicleService(
        IRepositoryBase<Vehicle> vehicleRepo,
        IRepositoryBase<Customer> customerRepo)
    {
        _vehicleRepo = vehicleRepo;
        _customerRepo = customerRepo;
    }

    public async Task<VehicleResponseDTO> RegisterVehicleAsync(RegisterVehicleDTO dto)
    {
        var customer = await _customerRepo.GetByIdAsync(dto.CustomerId)
            ?? throw new KeyNotFoundException($"Customer {dto.CustomerId} not found.");

        // Prevent duplicate plate number
        var existing = await _vehicleRepo
            .FindByCondition(v => v.VehicleNumber == dto.VehicleNumber)
            .FirstOrDefaultAsync();

        if (existing is not null)
            throw new InvalidOperationException($"Vehicle number '{dto.VehicleNumber}' is already registered.");

        var vehicle = new Vehicle
        {
            CustomerId = dto.CustomerId,
            VehicleNumber = dto.VehicleNumber,
            Brand = dto.Brand,
            Model = dto.Model,
            Color = dto.Color,
            Year = dto.Year
        };

        _vehicleRepo.Create(vehicle);
        await _vehicleRepo.SaveChangesAsync();

        return Map(vehicle);
    }

    public async Task<List<VehicleResponseDTO>> GetVehiclesByCustomerAsync(int customerId)
    {
        var vehicles = await _vehicleRepo
            .FindByCondition(v => v.CustomerId == customerId)
            .ToListAsync();

        return vehicles.Select(Map).ToList();
    }

    public async Task<VehicleResponseDTO?> GetVehicleByIdAsync(int vehicleId)
    {
        var v = await _vehicleRepo.GetByIdAsync(vehicleId);
        return v is null ? null : Map(v);
    }

    public async Task<bool> DeleteVehicleAsync(int vehicleId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
        if (vehicle is null) return false;

        _vehicleRepo.Delete(vehicle);
        await _vehicleRepo.SaveChangesAsync();
        return true;
    }

    private static VehicleResponseDTO Map(Vehicle v) => new()
    {
        VehicleId = v.VehicleId,
        CustomerId = v.CustomerId,
        VehicleNumber = v.VehicleNumber,
        Brand = v.Brand,
        Model = v.Model,
        Color = v.Color,
        Year = v.Year
    };
}