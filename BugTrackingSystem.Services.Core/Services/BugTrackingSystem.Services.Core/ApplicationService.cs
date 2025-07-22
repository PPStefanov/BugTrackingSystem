using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Services.Core
{
    public class ApplicationService : IApplicationService
    {
        private readonly BugTrackingSystemDbContext _context;

        public ApplicationService(BugTrackingSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationName>> GetAllApplicationsAsync()
        {
            return await _context.ApplicationName
                .Include(a => a.BugReports)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<ApplicationName?> GetApplicationByIdAsync(int id)
        {
            return await _context.ApplicationName
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ApplicationName?> GetApplicationWithBugReportsAsync(int id)
        {
            return await _context.ApplicationName
                .Include(a => a.BugReports)
                .ThenInclude(b => b.Status)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ApplicationName> CreateApplicationAsync(ApplicationName application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            // Set default values
            application.IsActive = true;
            application.CreatedAt = DateTime.Now;
            application.UpdatedAt = null;

            _context.ApplicationName.Add(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<ApplicationName?> UpdateApplicationAsync(ApplicationName application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            var existingApp = await _context.ApplicationName.FindAsync(application.Id);
            if (existingApp == null)
                return null;

            // Update properties
            existingApp.Name = application.Name;
            existingApp.Description = application.Description;
            existingApp.IsActive = application.IsActive;
            existingApp.UpdatedAt = DateTime.Now;

            _context.ApplicationName.Update(existingApp);
            await _context.SaveChangesAsync();

            return existingApp;
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var application = await _context.ApplicationName
                .Include(a => a.BugReports)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return false;

            // Check if application has associated bug reports
            if (application.BugReports.Any())
                return false; // Cannot delete application with bug reports

            _context.ApplicationName.Remove(application);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleApplicationStatusAsync(int id)
        {
            var application = await _context.ApplicationName.FindAsync(id);
            if (application == null)
                return false;

            application.IsActive = !application.IsActive;
            application.UpdatedAt = DateTime.Now;

            _context.ApplicationName.Update(application);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ApplicationExistsAsync(int id)
        {
            return await _context.ApplicationName.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> CanDeleteApplicationAsync(int id)
        {
            var application = await _context.ApplicationName
                .Include(a => a.BugReports)
                .FirstOrDefaultAsync(a => a.Id == id);

            return application != null && !application.BugReports.Any();
        }

        public async Task<int> GetBugReportCountAsync(int applicationId)
        {
            return await _context.BugReports
                .CountAsync(b => b.ApplicationId == applicationId);
        }
    }
}