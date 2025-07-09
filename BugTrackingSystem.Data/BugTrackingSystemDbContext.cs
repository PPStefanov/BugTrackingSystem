using BugTrackingSystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BugTrackingSystem.Data
{
    public class BugTrackingSystemDbContext : IdentityDbContext<AppUser>
    {
        public BugTrackingSystemDbContext(DbContextOptions<BugTrackingSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<BugReport> BugReports { get; set; }
        public DbSet<ApplicationName> ApplicationName { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<BugPriorityEntity> BugPriorities { get; set; }
        public DbSet<BugStatusEntity> BugStatuses { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
