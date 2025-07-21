using Microsoft.AspNetCore.Identity;
using SurveyBD.Models;

namespace SurveyBD.Data
{
    public class DbInitializer
    {
        public static async Task SeedAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Create Admin role if it doesn't exist
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Create Admin user
                string adminEmail = "admin@surveybd.com";
                string adminPassword = "Admin@123";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log this somewhere (optional)
                Console.WriteLine("Error seeding admin: " + ex.Message);
                // Allow the app to continue running without crashing
            }
        }
    }
}
