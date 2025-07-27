using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BugTrackingSystem.Data
{
    public class BugTrackingSystemDbContextFactory : IDesignTimeDbContextFactory<BugTrackingSystemDbContext>
    {
        public BugTrackingSystemDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BugTrackingSystem.Web"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BugTrackingSystemDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            optionsBuilder.UseSqlServer(connectionString);

            return new BugTrackingSystemDbContext(optionsBuilder.Options);
        }
    }
}
