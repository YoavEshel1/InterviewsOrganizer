using InterviewsOrganizer.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace InterviewsOrganizer.Data
{
    public static class DbSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles if they don't exist
            string[] roles = ["SuperAdmin", "Admin", "User"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Create super admin if doesn't exist
            var superAdminEmail = "superadmin@interviewsorganizer.com";
            var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdmin is null)
            {
                superAdmin = new AppUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(superAdmin, "SuperAdmin@123!");
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }
    }
}