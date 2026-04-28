using VehicleIMS.Application.DTOs.Purchase;

namespace VehicleIMS.Application.Interfaces;

public interface IPurchaseService
{
    Task<IEnumerable<PurchaseResponseDto>> GetAllInvoicesAsync();
    Task<PurchaseResponseDto?> GetInvoiceByIdAsync(int id);
    Task<PurchaseResponseDto> CreateInvoiceAsync(CreatePurchaseDto dto);
}