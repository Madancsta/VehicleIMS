using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Application.DTOs
{
    public class PartRequestDTO
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public int PartId { get; set; }

        [Range(1, 100)]
        public int RequestQuantity { get; set; }

        [Required]
        public string RequestDescription { get; set; } = string.Empty;
    }

}