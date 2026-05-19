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
    private readonly IRepositoryBase<Request> _requestRepo;
    private readonly ICustomerRepository _customerRepository;
    private readonly IRequestRepository _requestRepository;
    private readonly IRepositoryBase<Service> _serviceRepo;
    private readonly IRepositoryBase<Vehicle> _vehicleRepo;
    private readonly IBookingRepository _bookingRepo;

    public SalesService(
        ISalesRepository salesRepo,
        IRepositoryBase<Part> partRepo,
        IRepositoryBase<Customer> customerRepo,
        IRepositoryBase<Request> requestRepo,
        ICustomerRepository customerRepository,
        IRequestRepository requestRepository,
        IRepositoryBase<Service> serviceRepo,
        IRepositoryBase<Vehicle> vehicleRepo,
        IBookingRepository bookingRepo)
    {
        _salesRepo = salesRepo;
        _partRepo = partRepo;
        _customerRepo = customerRepo;
        _requestRepo = requestRepo;
        _customerRepository = customerRepository;
        _requestRepository = requestRepository;
        _serviceRepo = serviceRepo;
        _vehicleRepo = vehicleRepo;
        _bookingRepo = bookingRepo;
    }

    // ── Create Sale ──────────────────────────────────────────────────────────

    public async Task<SalesResponseDTO> CreateSaleAsync(CreateSalesDTO dto)
    {
        // Validate customer
        var customer = await _customerRepo.GetByIdAsync(dto.CustomerId)
            ?? throw new KeyNotFoundException($"Customer {dto.CustomerId} not found.");

        // Validate service 
        Service? service = null;
        if (dto.ServiceId.HasValue)
        {
            service = await _serviceRepo.GetByIdAsync(dto.ServiceId.Value)
                ?? throw new KeyNotFoundException($"Service {dto.ServiceId} not found.");
        }

        // Validate vehicle
        Vehicle? vehicle = null;
        if (dto.VehicleId.HasValue)
        {
            vehicle = await _vehicleRepo.GetByIdAsync(dto.VehicleId.Value)
                ?? throw new KeyNotFoundException($"Vehicle {dto.VehicleId} not found.");

            if (vehicle.CustomerId != dto.CustomerId)
                throw new InvalidOperationException("Vehicle does not belong to this customer.");
        }

        // Validate Booking if provided
        Booking? booking = null;
        if (dto.BookingId.HasValue && dto.BookingId.Value > 0)
        {
            booking = await _bookingRepo.GetByIdWithDetailsAsync(dto.BookingId.Value);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking {dto.BookingId} not found.");
            }
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

        // Loyalty Program: 10% discount only when single purchase is more than 5000
        decimal discount = 0m;

        if (subtotal > 5000m)
        {
            discount = Math.Round(subtotal * 0.10m, 2);
        }

        decimal total = subtotal - discount;

        // Determine payment status based on payment method
        PaymentStatus paymentStatus;
        bool isCreditPayment = dto.PaymentMethod.Equals("credit", StringComparison.OrdinalIgnoreCase);

        if (isCreditPayment)
        {
            paymentStatus = PaymentStatus.Pending;
        }
        else
        {
            paymentStatus = PaymentStatus.Completed;
        }

        // Update customer's financial records
        if (paymentStatus == PaymentStatus.Completed)
        {
            customer.TotalSpent = (customer.TotalSpent ?? 0f) + (float)total;

            int earnedPoints = (int)(total / 100);
            customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + earnedPoints;
        }
        else if (isCreditPayment)
        {
            customer.CreditBalance = (customer.CreditBalance ?? 0f) + (float)total;
        }

        // Update customer in database
        await _customerRepository.UpdateAsync(customer);

        // Generate invoice number
        string invoiceNumber = await _salesRepo.GenerateInvoiceNumberAsync();

        var sale = new Sales
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = dto.CustomerId,
            ServiceId = dto.ServiceId,
            BookingId = dto.BookingId.HasValue && dto.BookingId.Value > 0 ? dto.BookingId.Value : (int?)null,
            PartsTotal = partsTotal,
            ServiceCharge = serviceCharge,
            Discount = discount,
            SalesAmount = total,
            PaymentStatus = paymentStatus,
            PaymentMethod = dto.PaymentMethod,
            SalesItems = salesItems
        };

        _salesRepo.Create(sale);
        await _salesRepo.SaveChangesAsync();

        if (booking != null)
        {
            // Complete booking
            booking.BookingStatus = BookingStatus.Completed;
            await _bookingRepo.SaveChangesAsync();

            // Complete related request
            var request = await _requestRepository.GetByBookingIdAsync(booking.BookingId);

            if (request != null)
            {
                request.RequestStatusId = 4;

                _requestRepo.Update(request);
                await _requestRepo.SaveChangesAsync();
            }
        }

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
            .Include(s => s.Booking)
                .ThenInclude(b => b.Vehicle)
            .Include(s => s.SalesItems).ThenInclude(si => si.Part)
            .ToListAsync();

        return sales.Select(MapToResponse).ToList();
    }

    public async Task<SalesResponseDTO> UpdateSalePaymentStatusAsync(int salesId, UpdateSalesStatusDTO dto)
    {
        // 1. Fetch existing sale with required navigations
        var sale = await _salesRepo.GetByIdWithDetailsAsync(salesId);
        if (sale == null)
            throw new KeyNotFoundException($"Sale {salesId} not found.");

        // 2. Validate transition (only Pending -> Completed is allowed for this example)
        if (sale.PaymentStatus == PaymentStatus.Pending && dto.PaymentStatus == PaymentStatus.Completed)
        {
            // Ensure payment method is "credit" (original assumption)
            if (!sale.PaymentMethod.Equals("credit", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only credit sales can be moved from Pending to Completed.");

            // Update customer financials
            var customer = await _customerRepo.GetByIdAsync(sale.CustomerId)
                ?? throw new KeyNotFoundException($"Customer {sale.CustomerId} not found.");

            // Move from CreditBalance to TotalSpent
            decimal saleAmount = sale.SalesAmount;
            customer.CreditBalance = (customer.CreditBalance ?? 0f) - (float)saleAmount;
            customer.TotalSpent = (customer.TotalSpent ?? 0f) + (float)saleAmount;

            // Award loyalty points (1 point per 100 spent)
            int earnedPoints = (int)(saleAmount / 100);
            customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + earnedPoints;

            await _customerRepository.UpdateAsync(customer);
        }
        else if (sale.PaymentStatus == dto.PaymentStatus)
        {
            // No change needed
            return MapToResponse(sale);
        }
        else
        {
            throw new InvalidOperationException($"Cannot change payment status from {sale.PaymentStatus} to {dto.PaymentStatus}.");
        }

        // 3. Update sale fields
        sale.PaymentStatus = dto.PaymentStatus;
        if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
            sale.PaymentMethod = dto.PaymentMethod;

        _salesRepo.Update(sale);
        await _salesRepo.SaveChangesAsync();

        // 4. Reload and return updated DTO
        var updated = await _salesRepo.GetByIdWithDetailsAsync(salesId)
            ?? throw new Exception("Failed to reload updated sale.");

        return MapToResponse(updated);
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
        BookingId = s.BookingId,
        VehicleId = s.VehicleId ?? s.Booking?.VehicleId,
        VehicleInfo = s.Vehicle != null
            ? $"{s.Vehicle.Brand} {s.Vehicle.Model} ({s.Vehicle.Year})"
            : (s.Booking?.Vehicle != null
                ? $"{s.Booking.Vehicle.Brand} {s.Booking.Vehicle.Model} ({s.Booking.Vehicle.Year})"
                : null),
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
