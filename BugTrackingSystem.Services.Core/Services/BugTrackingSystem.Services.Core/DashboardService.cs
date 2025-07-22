using BugTrackingSystem.Data;
using BugTrackingSystem.ViewModels.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Services.Core
{
    public class DashboardService : IDashboardService
    {
        private readonly BugTrackingSystemDbContext _context;

        public DashboardService(BugTrackingSystemDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;
            var weekAgo = now.AddDays(-7);
            var monthAgo = now.AddDays(-30);

            // Current period stats
            var totalBugReports = await _context.BugReports
                .Where(b => b.IsActive)
                .CountAsync();

            var resolvedIssues = await _context.BugReports
                .Where(b => b.IsActive && b.Status.Name == "Closed")
                .CountAsync();

            var pendingReview = await _context.BugReports
                .Where(b => b.IsActive && (b.Status.Name == "Ready for Regression" || b.Status.Name == "In Test"))
                .CountAsync();

            var activeUsers = await _context.Users
                .Where(u => u.ReportedBugs.Any(b => b.CreatedAt >= monthAgo) || 
                           u.AssignedBugs.Any(b => b.UpdatedAt >= monthAgo))
                .CountAsync();

            var newThisWeek = await _context.BugReports
                .Where(b => b.IsActive && b.CreatedAt >= weekAgo)
                .CountAsync();

            var inProgress = await _context.BugReports
                .Where(b => b.IsActive && b.Status.Name == "In Development")
                .CountAsync();

            var highPriority = await _context.BugReports
                .Where(b => b.IsActive && b.Priority.Name == "High")
                .CountAsync();

            var overdueIssues = await _context.BugReports
                .Where(b => b.IsActive && 
                           b.CreatedAt <= now.AddDays(-30) && 
                           b.Status.Name != "Closed")
                .CountAsync();

            // Previous period stats for comparison
            var previousMonthStart = monthAgo.AddDays(-30);
            var prevTotalBugReports = await _context.BugReports
                .Where(b => b.IsActive && b.CreatedAt >= previousMonthStart && b.CreatedAt < monthAgo)
                .CountAsync();

            var prevResolvedIssues = await _context.BugReports
                .Where(b => b.IsActive && 
                           b.Status.Name == "Closed" && 
                           b.UpdatedAt >= previousMonthStart && 
                           b.UpdatedAt < monthAgo)
                .CountAsync();

            return new DashboardStatsViewModel
            {
                TotalBugReports = totalBugReports,
                ResolvedIssues = resolvedIssues,
                PendingReview = pendingReview,
                ActiveUsers = activeUsers,
                NewThisWeek = newThisWeek,
                InProgress = inProgress,
                HighPriority = highPriority,
                OverdueIssues = overdueIssues,
                TotalBugReportsChange = CalculatePercentageChange(prevTotalBugReports, totalBugReports),
                ResolvedIssuesChange = CalculatePercentageChange(prevResolvedIssues, resolvedIssues)
            };
        }

        public async Task<List<RecentActivityViewModel>> GetRecentActivityAsync(int count = 10)
        {
            var recentBugs = await _context.BugReports
                .Include(b => b.Reporter)
                .Include(b => b.Status)
                .Include(b => b.Priority)
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.UpdatedAt ?? b.CreatedAt)
                .Take(count)
                .Select(b => new RecentActivityViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Action = b.UpdatedAt.HasValue ? "Updated" : "Created",
                    UserName = b.Reporter.UserName ?? "Unknown",
                    Timestamp = b.UpdatedAt ?? b.CreatedAt,
                    Status = b.Status.Name,
                    Priority = b.Priority.Name,
                    ActivityType = b.UpdatedAt.HasValue ? "Updated" : "Created"
                })
                .ToListAsync();

            return recentBugs;
        }

        public async Task<Dictionary<string, int>> GetBugsByStatusAsync()
        {
            var bugsByStatus = await _context.BugReports
                .Include(b => b.Status)
                .Where(b => b.IsActive)
                .GroupBy(b => b.Status.Name)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            return bugsByStatus;
        }

        public async Task<Dictionary<string, int>> GetBugsByPriorityAsync()
        {
            var bugsByPriority = await _context.BugReports
                .Include(b => b.Priority)
                .Where(b => b.IsActive)
                .GroupBy(b => b.Priority.Name)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Priority, x => x.Count);

            return bugsByPriority;
        }

        public async Task<List<MonthlyStatsViewModel>> GetMonthlyStatsAsync(int months = 6)
        {
            var startDate = DateTime.Now.AddMonths(-months);
            var monthlyStats = new List<MonthlyStatsViewModel>();

            for (int i = 0; i < months; i++)
            {
                var monthStart = startDate.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1);

                var created = await _context.BugReports
                    .Where(b => b.CreatedAt >= monthStart && b.CreatedAt < monthEnd)
                    .CountAsync();

                var resolved = await _context.BugReports
                    .Where(b => b.Status.Name == "Closed" && 
                               b.UpdatedAt >= monthStart && 
                               b.UpdatedAt < monthEnd)
                    .CountAsync();

                var inProgress = await _context.BugReports
                    .Where(b => b.Status.Name == "In Development" && 
                               b.CreatedAt >= monthStart && 
                               b.CreatedAt < monthEnd)
                    .CountAsync();

                monthlyStats.Add(new MonthlyStatsViewModel
                {
                    Month = monthStart.ToString("MMM"),
                    Year = monthStart.Year,
                    Created = created,
                    Resolved = resolved,
                    InProgress = inProgress
                });
            }

            return monthlyStats;
        }

        private double CalculatePercentageChange(int oldValue, int newValue)
        {
            if (oldValue == 0) return newValue > 0 ? 100 : 0;
            return ((double)(newValue - oldValue) / oldValue) * 100;
        }
    }
}