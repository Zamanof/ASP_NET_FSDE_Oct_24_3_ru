using ASP_NET_21._TaskFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ASP_NET_21._TaskFlow.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[] { "Admin", "Manager", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "admin@taskflow.com";
        var adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Nadir",
                LastName = "Zamanov",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

}
