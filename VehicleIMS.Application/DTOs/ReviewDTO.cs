using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleIMS.Application.DTOs
{
    public class ReviewDTO
    {
        public int SalesId { get; set; }

        public int Rating { get; set; }

        public string ReviewComment { get; set; } = string.Empty;
    }
}