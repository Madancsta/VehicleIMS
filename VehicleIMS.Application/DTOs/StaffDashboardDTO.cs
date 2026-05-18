using System.Collections.Generic;

namespace VehicleIMS.Application.DTOs
{
    public class StaffDashboardStatsDTO
    {
        public int TodaySalesCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public int ActiveCustomersCount { get; set; }
        public int PendingBookingsCount { get; set; }
        public int LowStockCount { get; set; }
    }

    public class StaffDashboardDataDTO
    {
        public StaffDashboardStatsDTO Stats { get; set; } = new();
        public List<RecentSaleDTO> RecentSales { get; set; } = new();
        public List<QuickActionDTO> QuickActions { get; set; } = new();
    }

    public class QuickActionDTO
    {
        public string Path { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}