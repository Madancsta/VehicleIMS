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

}
