using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Domain.Enums;

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

    private static async Task SeedUserAsync(
        UserManager<Users> userManager,
        string username,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user != null) return;

        var newUser = new Users
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Email = email,
            PhoneNumber = "9800000000",
            Address = "Kathmandu",
            CreatedAt = DateTime.UtcNow,
            Status = UserStatus.Active,
            EmailConfirmed = true
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