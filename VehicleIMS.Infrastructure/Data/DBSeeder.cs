using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;
using VehicleIMS.Infrastructure.Data;

public static class DBSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new Role { Name = "Admin", NormalizedName = "ADMIN" });
        }

        var adminEmail = "admin@vehicleims.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new Users
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Email = adminEmail,
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.Active
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create admin user: {errors}");
            }
        }
    }

    
}