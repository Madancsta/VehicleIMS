using System;
using System.Collections.Generic;

namespace VehicleIMS.Application.DTOs
{
    public class AdminDashboardStatsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int ActiveCustomersCount { get; set; }
        public int TotalPartsCount { get; set; }
        public int LowStockCount { get; set; }
        public decimal OutstandingCredit { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    public class MonthlyRevenueDTO
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public int Year { get; set; }
        public int MonthNumber { get; set; }
    }

    public class AlertDTO
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }

    public class AdminDashboardDataDTO
    {
        public AdminDashboardStatsDTO Stats { get; set; } = new();
        public List<MonthlyRevenueDTO> MonthlyRevenue { get; set; } = new();
        public List<AlertDTO> Alerts { get; set; } = new();
        public List<RecentSaleDTO> RecentSales { get; set; } = new();
        public List<TopCustomerDTO> TopCustomers { get; set; } = new();
    }

    public class TopCustomerDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int TotalPurchases { get; set; }
    }
}