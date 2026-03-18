namespace FreshNFluffy.Common
{
    using Microsoft.AspNetCore.Identity;
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Administrator", "User" };

            foreach(string role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
