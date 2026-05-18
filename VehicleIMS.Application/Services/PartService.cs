using VehicleIMS.Application.DTOs.Part;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services;

public class PartService(IPartRepository partRepository) : IPartService
{
    public async Task<IEnumerable<PartResponseDto>> GetAllPartsAsync()
    {
        var parts = await partRepository.GetAllWithCategoryAsync();
        return parts.Select(MapToResponse);
    }

    public async Task<PartResponseDto?> GetPartByIdAsync(int id)
    {
        var part = await partRepository.GetByIdWithCategoryAsync(id);
        return part is null ? null : MapToResponse(part);
    }

    public async Task<PartResponseDto> CreatePartAsync(CreatePartDto dto)
    {
        var part = new Part
        {
            PartName = dto.PartName,
            PartCategoryId = dto.PartCategoryId,
            PartPrice = dto.PartPrice,
            StockQuantity = 0
        };

        partRepository.Create(part);
        await partRepository.SaveChangesAsync();
        return MapToResponse(part);
    }

    public async Task<PartResponseDto?> UpdatePartAsync(int id, UpdatePartDto dto)
    {
        var part = await partRepository.GetByIdAsync(id);
        if (part is null) return null;

        part.PartName = dto.PartName;
        part.PartCategoryId = dto.PartCategoryId;
        part.PartPrice = dto.PartPrice;


        partRepository.Update(part);
        await partRepository.SaveChangesAsync();
        return MapToResponse(part);
    }

    public async Task<bool> DeletePartAsync(int id)
    {
        var part = await partRepository.GetByIdAsync(id);
        if (part is null) return false;

        partRepository.Delete(part);
        await partRepository.SaveChangesAsync();
        return true;
    }

    private static PartResponseDto MapToResponse(Part part) => new()
    {
        PartId = part.PartId,
        PartName = part.PartName,
        PartCategoryId = part.PartCategoryId,
        CategoryName = part.PartCategory?.CategoryName ?? string.Empty,
        PartPrice = part.PartPrice,
        StockQuantity = part.StockQuantity
    };
}