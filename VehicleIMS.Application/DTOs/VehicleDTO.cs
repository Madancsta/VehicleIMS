using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleIMS.Application.DTOs
{
    public class VehicleDTO
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
