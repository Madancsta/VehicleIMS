using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Application.DTOs
{
    public class ReviewDTO
    {
        [Required]
        public int SalesId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string ReviewComment { get; set; } = string.Empty;
    }

    public class ReviewableSaleDTO
    {
        public int SalesId { get; set; }
        public int BookingId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public DateTime SalesDate { get; set; }
        public decimal SalesAmount { get; set; }
    }


}