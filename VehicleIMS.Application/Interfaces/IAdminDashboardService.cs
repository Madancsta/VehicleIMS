using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardStatsDTO> GetDashboardStatsAsync();
        Task<List<MonthlyRevenueDTO>> GetMonthlyRevenueAsync(int months = 6);
        Task<List<AlertDTO>> GetAlertsAsync();
        Task<List<RecentSaleDTO>> GetRecentSalesAsync(int count = 10);
        Task<List<TopCustomerDTO>> GetTopCustomersAsync(int count = 5);
        Task<AdminDashboardDataDTO> GetDashboardDataAsync();
        Task<decimal> GetOutstandingCreditAsync();
        Task<decimal> GetTotalExpensesAsync();
    }
}