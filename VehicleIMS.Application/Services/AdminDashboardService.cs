using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IRepositoryBase<Sales> _salesRepo;
        private readonly IRepositoryBase<Customer> _customerRepo;
        private readonly IRepositoryBase<Part> _partRepo;
        private readonly IRepositoryBase<Booking> _bookingRepo;
        private readonly IRepositoryBase<Purchase> _purchaseRepo;

        public AdminDashboardService(
            IRepositoryBase<Sales> salesRepo,
            IRepositoryBase<Customer> customerRepo,
            IRepositoryBase<Part> partRepo,
            IRepositoryBase<Booking> bookingRepo,
            IRepositoryBase<Purchase> purchaseRepo)
        {
            _salesRepo = salesRepo;
            _customerRepo = customerRepo;
            _partRepo = partRepo;
            _bookingRepo = bookingRepo;
            _purchaseRepo = purchaseRepo;
        }

        public async Task<AdminDashboardStatsDTO> GetDashboardStatsAsync()
        {
            var allSales = await _salesRepo.FindAll().ToListAsync();
            var activeCustomers = await _customerRepo
                .FindByCondition(c => c.User.Status == UserStatus.Active)
                .CountAsync();
            var totalParts = await _partRepo.FindAll().CountAsync();
            var lowStockParts = await _partRepo
                .FindByCondition(p => p.StockQuantity < 10)
                .CountAsync();

            var outstandingCredit = allSales
                .Where(s => s.PaymentStatus != PaymentStatus.Completed)
                .Sum(s => s.SalesAmount);

            var totalExpenses = await _purchaseRepo
                .FindAll()
                .SumAsync(p => p.PurchasePrice);

            var totalRevenue = allSales.Sum(s => s.SalesAmount);
            var profitMargin = totalRevenue > 0 ? ((totalRevenue - totalExpenses) / totalRevenue) * 100 : 0;

            return new AdminDashboardStatsDTO
            {
                TotalRevenue = totalRevenue,
                ActiveCustomersCount = activeCustomers,
                TotalPartsCount = totalParts,
                LowStockCount = lowStockParts,
                OutstandingCredit = outstandingCredit,
                TotalExpenses = totalExpenses,
                ProfitMargin = profitMargin
            };
        }

        public async Task<List<MonthlyRevenueDTO>> GetMonthlyRevenueAsync(int months = 6)
        {
            var result = new List<MonthlyRevenueDTO>();

            // Use UTC date with Kind = Utc
            var today = DateTime.UtcNow;
            var startDate = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMonths(-months + 1);

            for (int i = 0; i < months; i++)
            {
                var monthDate = startDate.AddMonths(i);

                // Create UTC date range for the month with Kind = Utc
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endOfMonth = startOfMonth.AddMonths(1);

                var monthlySales = await _salesRepo
                    .FindByCondition(s => s.SalesDate >= startOfMonth && s.SalesDate < endOfMonth)
                    .ToListAsync();

                var monthlyExpenses = await _purchaseRepo
                    .FindByCondition(p => p.PurchaseDate >= startOfMonth && p.PurchaseDate < endOfMonth)
                    .ToListAsync();

                result.Add(new MonthlyRevenueDTO
                {
                    Month = monthDate.ToString("MMM"),
                    MonthNumber = monthDate.Month,
                    Year = monthDate.Year,
                    Revenue = monthlySales.Sum(s => s.SalesAmount),
                    Expenses = monthlyExpenses.Sum(p => p.PurchasePrice)
                });
            }

            return result;
        }

        public async Task<List<AlertDTO>> GetAlertsAsync()
        {
            var alerts = new List<AlertDTO>();
            var now = DateTime.UtcNow;

            // Low stock alerts
            var lowStockParts = await _partRepo
                .FindByCondition(p => p.StockQuantity < 10)
                .ToListAsync();

            foreach (var part in lowStockParts)
            {
                alerts.Add(new AlertDTO
                {
                    Id = part.PartId,
                    Message = $"Low stock alert: {part.PartName} has only {part.StockQuantity} units left",
                    Type = "LowStock",
                    CreatedAt = now,
                    TimeAgo = "Just now"
                });
            }

            // Pending bookings alerts
            var pendingBookings = await _bookingRepo
                .FindByCondition(b => b.BookingStatus == BookingStatus.Pending)
                .CountAsync();

            if (pendingBookings > 0)
            {
                alerts.Add(new AlertDTO
                {
                    Id = 999,
                    Message = $"You have {pendingBookings} pending booking(s) awaiting confirmation",
                    Type = "Booking",
                    CreatedAt = now,
                    TimeAgo = "Just now"
                });
            }

            // Outstanding credit alerts
            var outstandingCredit = await GetOutstandingCreditAsync();
            if (outstandingCredit > 10000)
            {
                alerts.Add(new AlertDTO
                {
                    Id = 998,
                    Message = $"High outstanding credit: Rs. {outstandingCredit:N0} needs attention",
                    Type = "Credit",
                    CreatedAt = now,
                    TimeAgo = "Just now"
                });
            }

            return alerts.OrderByDescending(a => a.CreatedAt).Take(10).ToList();
        }

        public async Task<List<RecentSaleDTO>> GetRecentSalesAsync(int count = 10)
        {
            var recentSales = await _salesRepo
                .FindAll()
                .Include(s => s.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(s => s.SalesDate)
                .Take(count)
                .ToListAsync();

            return recentSales.Select(s => new RecentSaleDTO
            {
                SalesId = s.SalesId,
                InvoiceNumber = s.InvoiceNumber,
                CustomerName = $"{s.Customer?.User?.FirstName} {s.Customer?.User?.LastName}".Trim(),
                SalesDate = s.SalesDate,
                SalesAmount = s.SalesAmount,
                PaymentStatus = s.PaymentStatus.ToString()
            }).ToList();
        }

        public async Task<List<TopCustomerDTO>> GetTopCustomersAsync(int count = 5)
        {
            var allSales = await _salesRepo
                .FindAll()
                .Include(s => s.Customer)
                    .ThenInclude(c => c.User)
                .ToListAsync();

            var topCustomers = allSales
                .GroupBy(s => new { s.CustomerId, s.Customer })
                .Select(g => new TopCustomerDTO
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = $"{g.Key.Customer?.User?.FirstName} {g.Key.Customer?.User?.LastName}".Trim(),
                    TotalSpent = g.Sum(s => s.SalesAmount),
                    TotalPurchases = g.Count()
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(count)
                .ToList();

            return topCustomers;
        }

        public async Task<AdminDashboardDataDTO> GetDashboardDataAsync()
        {
            var stats = await GetDashboardStatsAsync();
            var monthlyRevenue = await GetMonthlyRevenueAsync(6);
            var alerts = await GetAlertsAsync();
            var recentSales = await GetRecentSalesAsync(5);
            var topCustomers = await GetTopCustomersAsync(5);

            return new AdminDashboardDataDTO
            {
                Stats = stats,
                MonthlyRevenue = monthlyRevenue,
                Alerts = alerts,
                RecentSales = recentSales,
                TopCustomers = topCustomers
            };
        }

        public async Task<decimal> GetOutstandingCreditAsync()
        {
            var allSales = await _salesRepo
                .FindByCondition(s => s.PaymentStatus != PaymentStatus.Completed)
                .ToListAsync();

            return allSales.Sum(s => s.SalesAmount);
        }

        public async Task<decimal> GetTotalExpensesAsync()
        {
            return await _purchaseRepo
                .FindAll()
                .SumAsync(p => p.PurchasePrice);
        }
    }
}