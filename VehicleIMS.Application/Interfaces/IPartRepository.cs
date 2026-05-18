using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces;

public interface IPartRepository : IRepositoryBase<Part>
{
    Task<IEnumerable<Part>> GetAllWithCategoryAsync();
    Task<Part?> GetByIdWithCategoryAsync(int id);
}