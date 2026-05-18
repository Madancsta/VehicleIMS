using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Application.Services;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using VehicleIMS.Infrastructure.Repositories;
using VehicleIMS.Infrastructure.Repository;
using System.Text;

namespace VehicleIMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<Users>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JWT");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["ValidIssuer"],
                ValidAudience = jwtSettings["ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddScoped<ISalesRepository, SalesRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IStaffDashboardService, StaffDashboardService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IVehicleService, VehicleService>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Staff", "Admin"));
            options.AddPolicy("CustomerOrAdmin", policy => policy.RequireRole("Customer", "Admin"));
        });

        return services;
    }
}