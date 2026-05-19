using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleIMS.Domain.Enums;
using VehicleIMS.Infrastructure.Data;

namespace VehicleIMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOrAdmin")]
public class FinancialReportController : ControllerBase
{
    private readonly AppDbContext _context;

    public FinancialReportController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetFinancialReport()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .ThenInclude(c => c.User)
            .ToListAsync();

        var totalRevenue = sales.Sum(s => s.SalesAmount);
        var totalExpenses = sales.Sum(s => s.PartsTotal);
        var netProfit = totalRevenue - totalExpenses;

        var monthlyBreakdown = sales
            .GroupBy(s => new
            {
                s.SalesDate.Year,
                s.SalesDate.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Revenue = g.Sum(x => x.SalesAmount),
                Expenses = g.Sum(x => x.PartsTotal)
            })
            .ToList();

        var outstandingCredits = sales
            .Where(s => s.PaymentStatus == PaymentStatus.Pending || s.PaymentMethod == "Credit")
            .Select(s => new
            {
                Invoice = s.InvoiceNumber,
                Customer = $"{s.Customer.FirstName} {s.Customer.LastName}",
                Total = s.SalesAmount,
                Paid = s.PaymentStatus == PaymentStatus.Pending ? 0 : s.SalesAmount,
                Pending = s.PaymentStatus == PaymentStatus.Pending ? s.SalesAmount : 0
            })
            .ToList();

        return Ok(new
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit,
            MonthlyBreakdown = monthlyBreakdown,
            OutstandingCredits = outstandingCredits
        });
    }
}
