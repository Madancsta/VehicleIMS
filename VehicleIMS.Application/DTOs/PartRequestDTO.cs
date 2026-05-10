using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleIMS.Application.DTOs
{
    public class PartRequestDTO
    {
        public int BookingId { get; set; }
        public int PartId { get; set; }
        public int RequestQuantity { get; set; }
        public string RequestDescription { get; set; } = string.Empty;
    }
}
