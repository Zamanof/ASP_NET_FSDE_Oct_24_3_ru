using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Identity;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        foreach (var role in new[] { "Admin", "Manager", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        const string adminEmail = "admin@taskflow.com";
        const string adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
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
