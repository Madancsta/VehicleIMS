using VehicleIMS.Application.DTOs.Vendor;

namespace VehicleIMS.Application.Interfaces;

public interface IVendorService
{
    Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync();
    Task<VendorResponseDto?> GetVendorByIdAsync(int id);
    Task<VendorResponseDto> CreateVendorAsync(CreateVendorDto dto);
    Task<VendorResponseDto?> UpdateVendorAsync(int id, UpdateVendorDto dto);
    Task<bool> DeleteVendorAsync(int id);
}