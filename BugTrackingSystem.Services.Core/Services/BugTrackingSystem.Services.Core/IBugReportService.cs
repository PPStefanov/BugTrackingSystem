using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels.BugReport;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBugReportService
{
    Task<List<BugReport>> GetBugsForUserAsync(AppUser user);
    Task<List<BugReport>> GetAllBugsAsync();

    Task<BugReport> GetByIdAsync(int id);

    Task<BugReport> CreateAsync(BugReport bugReport, AppUser creator);

    Task<bool> UpdateAsync(BugReport bugReport, AppUser updater);
        
    Task<bool> DeleteAsync(int id, AppUser deleter);

    // Assignment methods
    Task<bool> AssignToUserAsync(int bugId, string assignedToUserId, AppUser assigner);

    // Dropdown data
    Task<List<BugPriorityEntity>> GetBugPrioritiesAsync();
    Task<List<ApplicationName>> GetApplicationsAsync();
    Task<List<BugStatusEntity>> GetBugStatusesAsync(); // New method
    Task<BugStatusEntity?> GetBugStatusByIdAsync(int statusId); // Get status by ID

    Task<List<BugReport>> GetBugsByStatusAsync(string statusName);

    Task<List<BugReportListViewModel>> GetAllBugReportViewModelsAsync();

    Task<bool> AutoAdvanceStatusToInTestIfQAAsync(BugReport bugReport, AppUser user, List<BugStatusEntity> statuses);
}