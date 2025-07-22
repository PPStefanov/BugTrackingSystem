using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Services.Core
{
    public interface IApplicationService
    {
        Task<List<ApplicationName>> GetAllApplicationsAsync();
        Task<ApplicationName?> GetApplicationByIdAsync(int id);
        Task<ApplicationName?> GetApplicationWithBugReportsAsync(int id);
        Task<ApplicationName> CreateApplicationAsync(ApplicationName application);
        Task<ApplicationName?> UpdateApplicationAsync(ApplicationName application);
        Task<bool> DeleteApplicationAsync(int id);
        Task<bool> ToggleApplicationStatusAsync(int id);
        Task<bool> ApplicationExistsAsync(int id);
        Task<bool> CanDeleteApplicationAsync(int id);
        Task<int> GetBugReportCountAsync(int applicationId);
    }
}