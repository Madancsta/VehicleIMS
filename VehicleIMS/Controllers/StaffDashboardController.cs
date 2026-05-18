using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
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
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving dashboard stats: {ex.Message}" });
            }
        }

        [HttpGet("recent-sales")]
        public async Task<IActionResult> GetRecentSales([FromQuery] int count = 10)
        {
            try
            {
                var recentSales = await _dashboardService.GetRecentSalesAsync(count);
                return Ok(recentSales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving recent sales: {ex.Message}" });
            }
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync();
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving dashboard data: {ex.Message}" });
            }
        }

        [HttpGet("today-sales-count")]
        public async Task<IActionResult> GetTodaySalesCount()
        {
            try
            {
                var count = await _dashboardService.GetTodaySalesCountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving today's sales count: {ex.Message}" });
            }
        }

        [HttpGet("today-revenue")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            try
            {
                var revenue = await _dashboardService.GetTodayRevenueAsync();
                return Ok(new { revenue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving today's revenue: {ex.Message}" });
            }
        }
    }
}