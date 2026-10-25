using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Data
{
    public static class DbSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var emailEncryptionService = serviceProvider.GetRequiredService<IEmailEncryptionService>();

            // Create roles if they don't exist
            string[] roles = ["SuperAdmin", "Admin", "User"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Create super admin if doesn't exist
            var superAdminEmail = "superadmin@interviewsorganizer.com";
            var superAdminEmailHash = emailEncryptionService.ComputeEmailHash(superAdminEmail);
            var superAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.EmailHash == superAdminEmailHash);
            if (superAdmin is null)
            {
                superAdmin = new AppUser
                {
                    UserName = $"user_{Guid.NewGuid()}",
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(superAdmin, "SuperAdmin@123!");
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }
    }
}