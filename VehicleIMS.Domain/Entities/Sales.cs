using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Domain.Entities;

public class Sales
{
    [Key]
    public int SalesId { get; set; }

    [Required]
    [StringLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public DateTime SalesDate { get; set; } = DateTime.UtcNow;

    // ── Customer ────────────────────────────────────────────────────────────
    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // ── Service
    [ForeignKey(nameof(Service))]
    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    // ── Financials ───────────────────────────────────────────────────────────
    [Column(TypeName = "decimal(18,2)")]
    public decimal PartsTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ServiceCharge { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SalesAmount { get; set; }

    // ── Payment ──────────────────────────────────────────────────────────────
    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Required]
    [StringLength(20)]
    public string PaymentMethod { get; set; } = "Cash"; // Cash | Card | Credit

    // ── Navigation ───────────────────────────────────────────────────────────
    public ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
    [ForeignKey(nameof(Booking))]
    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }

    [ForeignKey(nameof(Review))]
    public int? ReviewId { get; set; }
    public Review? Review { get; set; }

    [ForeignKey(nameof(Vehicle))]
    public int? VehicleId { get; set; }
    public virtual Vehicle? Vehicle { get; set; }

}
