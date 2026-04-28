using VehicleIMS.Application.DTOs.Part;

namespace VehicleIMS.Application.Interfaces;

public interface IPartService
{
    Task<IEnumerable<PartResponseDto>> GetAllPartsAsync();
    Task<PartResponseDto?> GetPartByIdAsync(int id);
    Task<PartResponseDto> CreatePartAsync(CreatePartDto dto);
    Task<PartResponseDto?> UpdatePartAsync(int id, UpdatePartDto dto);
    Task<bool> DeletePartAsync(int id);
}