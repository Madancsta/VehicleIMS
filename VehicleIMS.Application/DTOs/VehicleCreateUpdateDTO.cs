using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Application.DTOs
{
    public class VehicleCreateUpdateDTO
    {
        [Required]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = string.Empty;

        [Range(1990, 2100)]
        public int Year { get; set; }
    }

}
