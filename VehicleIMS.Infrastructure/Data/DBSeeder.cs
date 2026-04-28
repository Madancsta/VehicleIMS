using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;
using VehicleIMS.Infrastructure.Data;

public static class DBSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        await SeedRoleAsync(roleManager, "Admin");
        await SeedRoleAsync(roleManager, "Staff");
        await SeedRoleAsync(roleManager, "Customer");

        await SeedUserAsync(userManager, "admin", "admin@vehicleims.com", "Admin@123", "Admin");
        await SeedUserAsync(userManager, "staff", "staff@vehicleims.com", "Staff@123", "Staff");
        await SeedUserAsync(userManager, "customer", "customer@vehicleims.com", "Customer@123", "Customer");
    }

    private static async Task SeedRoleAsync(RoleManager<Role> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            });
        }
    }

        // Create admin user if not exists
        var adminEmail = "admin@vehicleims.com"; // Consistent email
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new Users
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Email = adminEmail, // Use the same email
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.Active
            };

        var result = await userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }

        await userManager.AddToRoleAsync(newUser, role);
    }

    
}