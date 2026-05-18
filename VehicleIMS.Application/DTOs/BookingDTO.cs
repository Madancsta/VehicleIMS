using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Application.DTOs
{
    public class BookingDTO
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public TimeSpan BookingTime { get; set; }

        [Required, StringLength(50)]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        public string ServiceDescription { get; set; } = string.Empty;
    }
    public class BookingResponseDTO
    {
        public int BookingId { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleInfo { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
    }

}
