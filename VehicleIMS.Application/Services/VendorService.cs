using VehicleIMS.Application.DTOs.Vendor;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services;

public class VendorService(IVendorRepository vendorRepository) : IVendorService
{
    public async Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync()
    {
        var vendors = await vendorRepository.FindAllAsync(trackChanges: false);
        return vendors.Select(MapToResponse);
    }

    public async Task<VendorResponseDto?> GetVendorByIdAsync(int id)
    {
        var vendor = await vendorRepository.GetByIdAsync(id);
        return vendor is null ? null : MapToResponse(vendor);
    }

    public async Task<VendorResponseDto> CreateVendorAsync(CreateVendorDto dto)
    {
        var vendor = new Vendor
        {
            VendorName = dto.VendorName,
            VendorEmail = dto.VendorEmail,
            VendorPhone = dto.VendorPhone,
            VendorAddress = dto.VendorAddress
        };

        vendorRepository.Create(vendor);
        await vendorRepository.SaveChangesAsync();
        return MapToResponse(vendor);
    }

    public async Task<VendorResponseDto?> UpdateVendorAsync(int id, UpdateVendorDto dto)
    {
        var vendor = await vendorRepository.GetByIdAsync(id);
        if (vendor is null) return null;

        vendor.VendorName = dto.VendorName;
        vendor.VendorEmail = dto.VendorEmail;
        vendor.VendorPhone = dto.VendorPhone;
        vendor.VendorAddress = dto.VendorAddress;

        vendorRepository.Update(vendor);
        await vendorRepository.SaveChangesAsync();
        return MapToResponse(vendor);
    }

    public async Task<bool> DeleteVendorAsync(int id)
    {
        var vendor = await vendorRepository.GetByIdAsync(id);
        if (vendor is null) return false;

        vendorRepository.Delete(vendor);
        await vendorRepository.SaveChangesAsync();
        return true;
    }

    private static VendorResponseDto MapToResponse(Vendor vendor) => new()
    {
        VendorId = vendor.VendorId,
        VendorName = vendor.VendorName,
        VendorEmail = vendor.VendorEmail,
        VendorPhone = vendor.VendorPhone,
        VendorAddress = vendor.VendorAddress
    };
}