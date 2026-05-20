using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs.Purchase;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Services;

public class PurchaseService(
    IPurchaseRepository purchaseRepository,
    IPartRepository partRepository) : IPurchaseService
{
    public async Task<IEnumerable<PurchaseResponseDto>> GetAllInvoicesAsync()
    {
        var purchases = await purchaseRepository
            .FindAll(trackChanges: false)
            .Include(p => p.PurchaseVendorParts)
                .ThenInclude(pvp => pvp.Part)
            .Include(p => p.PurchaseVendorParts)
                .ThenInclude(pvp => pvp.Vendor)
            .ToListAsync();

        return purchases.Select(MapToResponse);
    }

    public async Task<PurchaseResponseDto?> GetInvoiceByIdAsync(int id)
    {
        var purchase = await purchaseRepository
            .FindByCondition(p => p.PurchaseId == id)
            .Include(p => p.PurchaseVendorParts)
                .ThenInclude(pvp => pvp.Part)
            .Include(p => p.PurchaseVendorParts)
                .ThenInclude(pvp => pvp.Vendor)
            .FirstOrDefaultAsync();

        return purchase is null ? null : MapToResponse(purchase);
    }

    public async Task<PurchaseResponseDto> CreateInvoiceAsync(CreatePurchaseDto dto)
    {
        var totalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
        var totalQuantity = dto.Items.Sum(i => i.Quantity);

        var purchase = new Purchase
        {
            PurchaseDate = DateTime.UtcNow,
            PurchaseQuantity = totalQuantity,
            PurchasePrice = totalAmount,
            PurchaseVendorParts = dto.Items.Select(i => new PurchaseVendorPart
            {
                VendorId = i.VendorId,
                PartId = i.PartId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        purchaseRepository.Create(purchase);

        // update stock for each part
        foreach (var item in dto.Items)
        {
            var part = await partRepository.GetByIdAsync(item.PartId);
            if (part is not null)
            {
                part.StockQuantity += item.Quantity;
                part.LastStockUpdate = DateTime.UtcNow;  
                partRepository.Update(part);
            }
        }

        await purchaseRepository.SaveChangesAsync();

        return await GetInvoiceByIdAsync(purchase.PurchaseId) ?? MapToResponse(purchase);
    }

    private static PurchaseResponseDto MapToResponse(Purchase purchase) => new()
    {
        PurchaseId = purchase.PurchaseId,
        PurchaseDate = purchase.PurchaseDate,
        TotalAmount = purchase.PurchasePrice,
        Items = purchase.PurchaseVendorParts?.Select(i => new PurchaseItemResponseDto
        {
            PartId = i.PartId,
            PartName = i.Part?.PartName ?? string.Empty,
            VendorId = i.VendorId,
            VendorName = i.Vendor?.VendorName ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList() ?? new()
    };
}