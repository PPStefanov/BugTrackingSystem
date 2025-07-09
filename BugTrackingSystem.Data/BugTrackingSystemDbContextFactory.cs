using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BugTrackingSystem.Data
{
    public class BugTrackingSystemDbContextFactory : IDesignTimeDbContextFactory<BugTrackingSystemDbContext>
    {
        public BugTrackingSystemDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BugTrackingSystemDbContext>();
            // Replace with your actual connection string for design time
            //optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=BugTrackingSystem_072025;Trusted_Connection=True;MultipleActiveResultSets=true;encrypt=false");   HOME
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BugTrackingSystem_072025;Trusted_Connection=True;MultipleActiveResultSets=true;encrypt=false"); // WORK

            return new BugTrackingSystemDbContext(optionsBuilder.Options);
        }
    }
}
