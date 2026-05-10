using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services;

public class SalesService : ISalesService
{
    private readonly ISalesRepository _salesRepo;
    private readonly IRepositoryBase<Part> _partRepo;
    private readonly IRepositoryBase<Customer> _customerRepo;
    private readonly IRepositoryBase<Service> _serviceRepo;
    private readonly IRepositoryBase<Vehicle> _vehicleRepo;

    public SalesService(
        ISalesRepository salesRepo,
        IRepositoryBase<Part> partRepo,
        IRepositoryBase<Customer> customerRepo,
        IRepositoryBase<Service> serviceRepo,
        IRepositoryBase<Vehicle> vehicleRepo)
    {
        _salesRepo = salesRepo;
        _partRepo = partRepo;
        _customerRepo = customerRepo;
        _serviceRepo = serviceRepo;
        _vehicleRepo = vehicleRepo;
    }

    // ── Create Sale ──────────────────────────────────────────────────────────

    public async Task<SalesResponseDTO> CreateSaleAsync(CreateSalesDTO dto)
    {
        // Validate customer
        var customer = await _customerRepo.GetByIdAsync(dto.CustomerId)
            ?? throw new KeyNotFoundException($"Customer {dto.CustomerId} not found.");

        // Validate service (optional)
        Service? service = null;
        if (dto.ServiceId.HasValue)
        {
            service = await _serviceRepo.GetByIdAsync(dto.ServiceId.Value)
                ?? throw new KeyNotFoundException($"Service {dto.ServiceId} not found.");
        }

        // Validate vehicle (optional)
        Vehicle? vehicle = null;
        if (dto.VehicleId.HasValue)
        {
            vehicle = await _vehicleRepo.GetByIdAsync(dto.VehicleId.Value)
                ?? throw new KeyNotFoundException($"Vehicle {dto.VehicleId} not found.");

            if (vehicle.CustomerId != dto.CustomerId)
                throw new InvalidOperationException("Vehicle does not belong to this customer.");
        }

        // Must have at least items OR a service
        if (dto.Items.Count == 0 && service == null)
            throw new InvalidOperationException("A sale must contain at least one part item or a service.");

        // Build line items and calculate parts total
        var salesItems = new List<SalesItem>();
        decimal partsTotal = 0;

        foreach (var itemDto in dto.Items)
        {
            var part = await _partRepo.GetByIdAsync(itemDto.PartId)
                ?? throw new KeyNotFoundException($"Part {itemDto.PartId} not found.");

            if (part.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{part.PartName}'. Available: {part.StockQuantity}.");

            // Deduct stock
            part.StockQuantity -= itemDto.Quantity;
            _partRepo.Update(part);

            var lineTotal = part.PartPrice * itemDto.Quantity;
            partsTotal += lineTotal;

            salesItems.Add(new SalesItem
            {
                PartId = part.PartId,
                Quantity = itemDto.Quantity,
                UnitPrice = part.PartPrice
            });
        }

        decimal serviceCharge = service != null ? (decimal)service.ServiceCharge : 0m;
        decimal subtotal = partsTotal + serviceCharge;

        // 10% loyalty discount
        decimal discount = Math.Round(subtotal * 0.10m, 2);
        decimal total = subtotal - discount;

        // Generate invoice number
        string invoiceNumber = await _salesRepo.GenerateInvoiceNumberAsync();

        var sale = new Sales
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = dto.CustomerId,
            ServiceId = dto.ServiceId,
            PartsTotal = partsTotal,
            ServiceCharge = serviceCharge,
            Discount = discount,
            SalesAmount = total,
            PaymentStatus = PaymentStatus.Completed,
            PaymentMethod = dto.PaymentMethod,
            SalesItems = salesItems
        };

        _salesRepo.Create(sale);
        await _salesRepo.SaveChangesAsync();

        // Reload with details for response
        var created = await _salesRepo.GetByIdWithDetailsAsync(sale.SalesId)
            ?? throw new Exception("Failed to reload saved sale.");

        return MapToResponse(created);
    }

    // ── Queries ──────────────────────────────────────────────────────────────

    public async Task<SalesResponseDTO?> GetSaleByIdAsync(int salesId)
    {
        var sale = await _salesRepo.GetByIdWithDetailsAsync(salesId);
        return sale is null ? null : MapToResponse(sale);
    }

    public async Task<InvoiceSummaryDTO?> GetInvoiceSummaryAsync(int salesId)
    {
        var sale = await _salesRepo.GetByIdWithDetailsAsync(salesId);
        if (sale is null) return null;

        return new InvoiceSummaryDTO
        {
            InvoiceNumber = sale.InvoiceNumber,
            SalesDate = sale.SalesDate,
            CustomerName = $"{sale.Customer.User.FirstName} {sale.Customer.User.LastName}",
            CustomerEmail = sale.Customer.User.Email,
            CustomerPhone = sale.Customer.User.PhoneNumber,
            ServiceInfo = sale.Service is null
                ? null
                : $"{sale.Service.VehicleType} – {sale.Service.ServiceType}",
            PartsTotal = sale.PartsTotal,
            ServiceCharge = sale.ServiceCharge,
            Subtotal = sale.PartsTotal + sale.ServiceCharge,
            Discount = sale.Discount,
            Total = sale.SalesAmount,
            PaymentMethod = sale.PaymentMethod,
            PaymentStatus = sale.PaymentStatus.ToString(),
            Items = sale.SalesItems.Select(si => new SalesItemResponseDTO
            {
                SalesItemId = si.SalesItemId,
                PartId = si.PartId,
                PartName = si.Part.PartName,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                LineTotal = si.Quantity * si.UnitPrice
            }).ToList()
        };
    }

    public async Task<List<SalesResponseDTO>> GetSalesByCustomerAsync(int customerId)
    {
        var sales = await _salesRepo.GetByCustomerIdAsync(customerId);
        return sales.Select(MapToResponse).ToList();
    }

    public async Task<List<SalesResponseDTO>> GetAllSalesAsync()
    {
        // Fetch via repository with includes
        var sales = await _salesRepo
            .FindAll()
            .Include(s => s.Customer).ThenInclude(c => c.User)
            .Include(s => s.Service)
            .Include(s => s.SalesItems).ThenInclude(si => si.Part)
            .ToListAsync();

        return sales.Select(MapToResponse).ToList();
    }

    // ── Mapper ───────────────────────────────────────────────────────────────

    private static SalesResponseDTO MapToResponse(Sales s) => new()
    {
        SalesId = s.SalesId,
        InvoiceNumber = s.InvoiceNumber,
        SalesDate = s.SalesDate,
        CustomerId = s.CustomerId,
        CustomerName = $"{s.Customer.User.FirstName} {s.Customer.User.LastName}",
        CustomerEmail = s.Customer.User.Email,
        ServiceId = s.ServiceId,
        ServiceType = s.Service?.ServiceType,
        VehicleType = s.Service?.VehicleType.ToString(),
        PartsTotal = s.PartsTotal,
        ServiceCharge = s.ServiceCharge,
        Discount = s.Discount,
        SalesAmount = s.SalesAmount,
        PaymentStatus = s.PaymentStatus.ToString(),
        PaymentMethod = s.PaymentMethod,
        Items = s.SalesItems.Select(si => new SalesItemResponseDTO
        {
            SalesItemId = si.SalesItemId,
            PartId = si.PartId,
            PartName = si.Part.PartName,
            Quantity = si.Quantity,
            UnitPrice = si.UnitPrice,
            LineTotal = si.Quantity * si.UnitPrice
        }).ToList()
    };
}
