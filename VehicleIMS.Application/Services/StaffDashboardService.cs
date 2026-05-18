using Microsoft.EntityFrameworkCore;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.Services
{
    public class StaffDashboardService : IStaffDashboardService
    {
        private readonly IRepositoryBase<Sales> _salesRepo;
        private readonly IRepositoryBase<Customer> _customerRepo;
        private readonly IRepositoryBase<Booking> _bookingRepo;
        private readonly IRepositoryBase<Part> _partRepo;

        public StaffDashboardService(
            IRepositoryBase<Sales> salesRepo,
            IRepositoryBase<Customer> customerRepo,
            IRepositoryBase<Booking> bookingRepo,
            IRepositoryBase<Part> partRepo)
        {
            _salesRepo = salesRepo;
            _customerRepo = customerRepo;
            _bookingRepo = bookingRepo;
            _partRepo = partRepo;
        }

        public async Task<StaffDashboardStatsDTO> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var todaySales = await _salesRepo
                .FindByCondition(s => s.SalesDate >= today && s.SalesDate < tomorrow)
                .ToListAsync();

            var activeCustomers = await _customerRepo
                .FindByCondition(c => c.User.Status == UserStatus.Active)
                .CountAsync();

            var pendingBookings = await _bookingRepo
                .FindByCondition(b => b.BookingStatus == BookingStatus.Pending)
                .CountAsync();

            var lowStockParts = await _partRepo
                .FindByCondition(p => p.StockQuantity < 10)
                .CountAsync();

            return new StaffDashboardStatsDTO
            {
                TodaySalesCount = todaySales.Count,
                TodayRevenue = todaySales.Sum(s => s.SalesAmount),
                ActiveCustomersCount = activeCustomers,
                PendingBookingsCount = pendingBookings,
                LowStockCount = lowStockParts
            };
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

        public async Task<StaffDashboardDataDTO> GetDashboardDataAsync()
        {
            var stats = await GetDashboardStatsAsync();
            var recentSales = await GetRecentSalesAsync(10);

            var quickActions = new List<QuickActionDTO>
            {
                new() { Path = "/staff/sales", Label = "New Sale", Icon = "ShoppingCart" },
                new() { Path = "/staff/customer-register", Label = "Register Customer", Icon = "UserPlus" },
                new() { Path = "/staff/search", Label = "Search Customer", Icon = "Search" },
                new() { Path = "/staff/customers", Label = "Customer History", Icon = "Calendar" }
            };

            return new StaffDashboardDataDTO
            {
                Stats = stats,
                RecentSales = recentSales,
                QuickActions = quickActions
            };
        }

        public async Task<int> GetTodaySalesCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _salesRepo
                .FindByCondition(s => s.SalesDate >= today && s.SalesDate < tomorrow)
                .CountAsync();
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var todaySales = await _salesRepo
                .FindByCondition(s => s.SalesDate >= today && s.SalesDate < tomorrow)
                .ToListAsync();

            return todaySales.Sum(s => s.SalesAmount);
        }

        public async Task<int> GetActiveCustomersCountAsync()
        {
            return await _customerRepo
                .FindByCondition(c => c.User.Status == UserStatus.Active)
                .CountAsync();
        }

        public async Task<int> GetPendingBookingsCountAsync()
        {
            return await _bookingRepo
                .FindByCondition(b => b.BookingStatus == BookingStatus.Pending)
                .CountAsync();
        }
    }
}