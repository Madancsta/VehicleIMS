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
        public DbSet<PartCategory> PartCategories => Set<PartCategory>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseVendorPart> PurchaseVendorParts => Set<PurchaseVendorPart>();
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<VendorPart> VendorParts => Set<VendorPart>();
        public DbSet<VendorUser> VendorUsers => Set<VendorUser>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<RequestPart> RequestParts => Set<RequestPart>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Sales> Sales => Set<Sales>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<SalesItem> SalesItems => Set<SalesItem>();
        public DbSet<NotificationReadState> NotificationReadStates => Set<NotificationReadState>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // PurchaseVendorPart composite key
            modelBuilder.Entity<PurchaseVendorPart>()
                 .HasKey(pvp => new { pvp.PurchaseId, pvp.PartId, pvp.VendorId });

            // VendorPart composite key
            modelBuilder.Entity<VendorPart>()
                .HasKey(vp => new { vp.VendorId, vp.PartId });

            // VendorUser composite key
            modelBuilder.Entity<VendorUser>()
                .HasKey(vu => new { vu.VendorId, vu.UserId });

            // Request to Booking relationship
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId);

            // RequestPart composite key
            modelBuilder.Entity<RequestPart>()
                .HasKey(rp => new { rp.RequestId, rp.PartId });

            // RequestPart relationships
            modelBuilder.Entity<RequestPart>()
                .HasOne(rp => rp.Request)
                .WithMany(r => r.RequestParts)
                .HasForeignKey(rp => rp.RequestId);

            // RequestPart to Part relationship
            modelBuilder.Entity<RequestPart>()
                .HasOne(rp => rp.Part)
                .WithMany()
                .HasForeignKey(rp => rp.PartId);

            // SalesItem → Sales
            modelBuilder.Entity<SalesItem>()
                .HasOne(si => si.Sales)
                .WithMany(s => s.SalesItems)
                .HasForeignKey(si => si.SalesId)
                .OnDelete(DeleteBehavior.Cascade);

            // SalesItem → Part (no cascade delete – parts outlive sales)
            modelBuilder.Entity<SalesItem>()
                .HasOne(si => si.Part)
                .WithMany()
                .HasForeignKey(si => si.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sales → Service (nullable FK)
            modelBuilder.Entity<Sales>()
                .HasOne(s => s.Service)
                .WithMany()
                .HasForeignKey(s => s.ServiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique invoice number
            modelBuilder.Entity<Sales>()
                .HasIndex(s => s.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<NotificationReadState>()
                .HasIndex(n => new { n.UserId, n.NotificationKey })
                .IsUnique();
    }
}
