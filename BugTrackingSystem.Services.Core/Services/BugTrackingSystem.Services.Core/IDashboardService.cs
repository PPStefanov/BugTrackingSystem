using BugTrackingSystem.ViewModels.Dashboard;

namespace BugTrackingSystem.Services.Core
{
    public interface IDashboardService
    {
        Task<DashboardStatsViewModel> GetDashboardStatsAsync();
        Task<List<RecentActivityViewModel>> GetRecentActivityAsync(int count = 10);
        Task<Dictionary<string, int>> GetBugsByStatusAsync();
        Task<Dictionary<string, int>> GetBugsByPriorityAsync();
        Task<List<MonthlyStatsViewModel>> GetMonthlyStatsAsync(int months = 6);
    }
}