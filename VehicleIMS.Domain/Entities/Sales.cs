using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Domain.Entities
{
    public class Sales
    {
        [Key]
        public int SalesId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public DateTime SalesDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalesAmount { get; set; }

        [Required]
        public int PaymentStatusId { get; set; }

        // Navigation properties
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public Review Review { get; set; }
    }
}