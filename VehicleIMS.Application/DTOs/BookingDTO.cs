using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleIMS.Application.DTOs
{
    public class BookingDTO
    {
        public int VehicleId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public string ServiceDescription { get; set; } = string.Empty;
    }
}
