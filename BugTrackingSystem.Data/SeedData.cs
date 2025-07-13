using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Admin", "QA", "Developer", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Example: Seed one user per role if they don't exist
            var users = new[]
                    {
                        new { Email = "admin@demo.com", Role = "Admin", Password = "SuperSecret1!" },
                        new { Email = "qa@demo.com", Role = "QA", Password = "SuperSecret2!" },
                        new { Email = "dev@demo.com", Role = "Developer", Password = "SuperSecret3!" },
                        new { Email = "user@demo.com", Role = "User", Password = "SuperSecret4!" }
                    };
            foreach (var u in users)
            {
                var user = await userManager.FindByEmailAsync(u.Email);
                if (user == null)
                {
                    user = new AppUser { UserName = u.Email, Email = u.Email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, u.Password);
                }
                if (!await userManager.IsInRoleAsync(user, u.Role))
                {
                    await userManager.AddToRoleAsync(user, u.Role);
                }
            }
        }

        public static async Task SeedPrioritiesAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<BugTrackingSystemDbContext>();

            if (!dbContext.BugPriorities.Any())
            {
                var priorities = new[]
                {
                    new BugPriorityEntity { /*Id = 1, */Name = "Low" },
                    new BugPriorityEntity { /*Id = 2, */Name = "Medium" },
                    new BugPriorityEntity { /*Id = 3, */Name = "High" }
                };

                dbContext.BugPriorities.AddRange(priorities);
                await dbContext.SaveChangesAsync();
            }
        }


        public static async Task SeedApplicationsAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<BugTrackingSystemDbContext>();

            if (!dbContext.ApplicationName.Any())
            {
                var applications = new[]
                {
            new ApplicationName { /*Id = 1, */Name = "Web App", IsActive = true },
            new ApplicationName { /*Id = 2, */Name = "Mobile App", IsActive = true },
            new ApplicationName { /*Id = 3, */Name = "Desktop App", IsActive = true }
        };

                dbContext.ApplicationName.AddRange(applications);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedStatusesAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<BugTrackingSystemDbContext>();

            if (!dbContext.BugStatuses.Any())
            {
                var statuses = new[]
                {
            new BugStatusEntity { Name = "New" },
            new BugStatusEntity { Name = "In Test" },
            new BugStatusEntity { Name = "In Development" },
            new BugStatusEntity { Name = "Ready for Regression" },
            new BugStatusEntity { Name = "Closed" }
        };

                dbContext.BugStatuses.AddRange(statuses);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}