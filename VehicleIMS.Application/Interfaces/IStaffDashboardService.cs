using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces
{
    public interface IStaffDashboardService
    {
        Task<StaffDashboardStatsDTO> GetDashboardStatsAsync();
        Task<List<RecentSaleDTO>> GetRecentSalesAsync(int count = 10);
        Task<StaffDashboardDataDTO> GetDashboardDataAsync();
        Task<int> GetTodaySalesCountAsync();
        Task<decimal> GetTodayRevenueAsync();
        Task<int> GetActiveCustomersCountAsync();
        Task<int> GetPendingBookingsCountAsync();
    }
}