using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace FreshNFluffy.Data.Seeding
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager = serviceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            string adminEmail = "admin.freshnfluffy@gmail.com";
            string adminPassword = "Admin123!";

            IdentityUser? existingUser = await userManager.FindByEmailAsync(adminEmail);

            if (existingUser == null)
            {
                IdentityUser admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }
            }

        }
    }
}
