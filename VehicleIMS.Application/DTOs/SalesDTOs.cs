using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.DTOs;

// ── Request DTOs ─────────────────────────────────────────────────────────────

public class CreateSalesDTO
{
    [Required]
    public int CustomerId { get; set; }
    public int? ServiceId { get; set; }

    public int? VehicleId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required unless a service is selected.")]
    public List<SalesItemDTO> Items { get; set; } = new();

    [Required]
    [StringLength(20)]
    public string PaymentMethod { get; set; } = "Cash";
}

public class SalesItemDTO
{
    [Required]
    public int PartId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

// ── Response DTOs ────────────────────────────────────────────────────────────

public class SalesResponseDTO
{
    public int SalesId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SalesDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public int? VehicleId { get; set; }
    public string? VehicleInfo { get; set; }  // e.g. "Toyota Corolla (2020)"
    public int? ServiceId { get; set; }
    public string? ServiceType { get; set; }
    public string? VehicleType { get; set; }
    public decimal PartsTotal { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal Discount { get; set; }
    public decimal SalesAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public List<SalesItemResponseDTO> Items { get; set; } = new();
}

public class SalesItemResponseDTO
{
    public int SalesItemId { get; set; }
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class InvoiceSummaryDTO
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SalesDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? VehicleInfo { get; set; }
    public string? ServiceInfo { get; set; }
    public decimal PartsTotal { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public List<SalesItemResponseDTO> Items { get; set; } = new();
}