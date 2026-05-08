using Microsoft.AspNetCore.Identity;               // ← Added for AddIdentity
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleIMS.Application.Interfaces;
using VehicleIMS.Application.Services;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Repository;

namespace VehicleIMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        
        services.AddIdentity<Users, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Staff Service
        services.AddScoped<IStaffService, StaffService>();

        // Customer History Service
        services.AddScoped<ICustomerHistoryService, CustomerHistoryService>();

        // Customer History Repository
        services.AddScoped<ICustomerHistoryRepository, CustomerHistoryRepository>();

        return services;
    }
}y