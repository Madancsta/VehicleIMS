using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
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

        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int months = 6)
        {
            try
            {
                var monthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync(months);
                return Ok(monthlyRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving monthly revenue: {ex.Message}" });
            }
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            try
            {
                var alerts = await _dashboardService.GetAlertsAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving alerts: {ex.Message}" });
            }
        }

        [HttpGet("recent-sales")]
        public async Task<IActionResult> GetRecentSales([FromQuery] int count = 5)
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

        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int count = 5)
        {
            try
            {
                var topCustomers = await _dashboardService.GetTopCustomersAsync(count);
                return Ok(topCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving top customers: {ex.Message}" });
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

        [HttpGet("outstanding-credit")]
        public async Task<IActionResult> GetOutstandingCredit()
        {
            try
            {
                var credit = await _dashboardService.GetOutstandingCreditAsync();
                return Ok(new { outstandingCredit = credit });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving outstanding credit: {ex.Message}" });
            }
        }

        [HttpGet("total-expenses")]
        public async Task<IActionResult> GetTotalExpenses()
        {
            try
            {
                var expenses = await _dashboardService.GetTotalExpensesAsync();
                return Ok(new { totalExpenses = expenses });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving total expenses: {ex.Message}" });
            }
        }
    }
}