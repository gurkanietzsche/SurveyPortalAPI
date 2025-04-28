using Microsoft.AspNetCore.Identity;
using SurveyPortalAPI.Models;

namespace SurveyPortalAPI
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Rolleri oluştur
            string[] roles = { ApplicationRoles.Admin, ApplicationRoles.User, ApplicationRoles.Surveyor };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin kullanıcısı oluştur
            var adminEmail = "admin@surveyportal.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
                }
            }

            // Surveyor kullanıcısı oluştur
            var surveyorEmail = "surveyor@surveyportal.com";
            var surveyorUser = await userManager.FindByEmailAsync(surveyorEmail);

            if (surveyorUser == null)
            {
                surveyorUser = new ApplicationUser
                {
                    UserName = "surveyor",
                    Email = surveyorEmail,
                    FirstName = "Anketör",
                    LastName = "User",
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(surveyorUser, "Surveyor123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(surveyorUser, ApplicationRoles.Surveyor);
                }
            }
        }
    }
}