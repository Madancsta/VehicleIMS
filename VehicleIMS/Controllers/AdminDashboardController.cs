using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int months = 6)
        {
            var monthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync(months);
            return Ok(monthlyRevenue);
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            var alerts = await _dashboardService.GetAlertsAsync();
            return Ok(alerts);
        }

        [HttpGet("recent-sales")]
        public async Task<IActionResult> GetRecentSales([FromQuery] int count = 5)
        {
            var recentSales = await _dashboardService.GetRecentSalesAsync(count);
            return Ok(recentSales);
        }

        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int count = 5)
        {
            var topCustomers = await _dashboardService.GetTopCustomersAsync(count);
            return Ok(topCustomers);
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetDashboardData()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return Ok(dashboardData);
        }

        [HttpGet("outstanding-credit")]
        public async Task<IActionResult> GetOutstandingCredit()
        {
            var credit = await _dashboardService.GetOutstandingCreditAsync();
            return Ok(new { outstandingCredit = credit });
        }

        [HttpGet("total-expenses")]
        public async Task<IActionResult> GetTotalExpenses()
        {
            var expenses = await _dashboardService.GetTotalExpensesAsync();
            return Ok(new { totalExpenses = expenses });
        }
    }
}
