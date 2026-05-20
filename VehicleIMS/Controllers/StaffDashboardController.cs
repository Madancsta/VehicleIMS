using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class StaffDashboardController : ControllerBase
    {
        private readonly IStaffDashboardService _dashboardService;

        public StaffDashboardController(IStaffDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("recent-sales")]
        public async Task<IActionResult> GetRecentSales([FromQuery] int count = 10)
        {
            var recentSales = await _dashboardService.GetRecentSalesAsync(count);
            return Ok(recentSales);
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetDashboardData()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return Ok(dashboardData);
        }

        [HttpGet("today-sales-count")]
        public async Task<IActionResult> GetTodaySalesCount()
        {
            var count = await _dashboardService.GetTodaySalesCountAsync();
            return Ok(new { count });
        }

        [HttpGet("today-revenue")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            var revenue = await _dashboardService.GetTodayRevenueAsync();
            return Ok(new { revenue });
        }
    }
}
