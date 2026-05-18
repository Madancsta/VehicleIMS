using System;
using System.Collections.Generic;

namespace VehicleIMS.Application.DTOs
{
    public class DashboardStatsDTO
    {
        public int TotalSalesCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveCustomersCount { get; set; }
        public int PendingBookingsCount { get; set; }
    }

    public class RecentSaleDTO
    {
        public int SalesId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime SalesDate { get; set; }
        public decimal SalesAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}