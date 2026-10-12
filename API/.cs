using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace API
{
    public static class SeedAdmin
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ----- Create Admin -----
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@test.com",
                Role = "Admin"
            };

            if (await userManager.FindByNameAsync(adminUser.UserName) == null)
            {
                await userManager.CreateAsync(adminUser, "Admin@123");
                Console.WriteLine("Admin user created.");
            }
            else
            {
                Console.WriteLine("Admin already exists.");
            }
        }
    }
}
