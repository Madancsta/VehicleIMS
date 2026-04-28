using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;
using VehicleIMS.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace VehicleIMS.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<Users, Role, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseVendorPart> PurchaseVendorParts => Set<PurchaseVendorPart>();
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<VendorPart> VendorParts => Set<VendorPart>();
        public DbSet<VendorUser> VendorUsers => Set<VendorUser>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Sales> Sales => Set<Sales>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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