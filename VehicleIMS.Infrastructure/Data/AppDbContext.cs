using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Users> Users => Set<Users>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<VehicleCustomer> VehicleCustomers => Set<VehicleCustomer>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingVehicle> BookingVehicles => Set<BookingVehicle>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<PartCategory> PartCategories => Set<PartCategory>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseVendorPart> PurchaseVendorParts => Set<PurchaseVendorPart>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorPart> VendorParts => Set<VendorPart>();
    public DbSet<VendorUser> VendorUsers => Set<VendorUser>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestPart> RequestParts => Set<RequestPart>();
    public DbSet<RequestBooking> RequestBookings => Set<RequestBooking>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewSales> ReviewVehicles => Set<ReviewSales>();
    public DbSet<Sales> Sales => Set<Sales>();
    public DbSet<SalesBooking> SalesBookings => Set<SalesBooking>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PurchaseVendorPart composite key
        modelBuilder.Entity<PurchaseVendorPart>()
            .HasKey(pvp => new { pvp.PurchaseId, pvp.PartId, pvp.VendorId });

        // VendorPart composite key
        modelBuilder.Entity<VendorPart>()
            .HasKey(vp => new { vp.VendorId, vp.PartId });

        modelBuilder.Entity<VendorUser>()
            .HasKey(vu => new { vu.VendorId, vu.UserId });
    }
}