using BugTrackingSystem.Data;
using BugTrackingSystem.ViewModels.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BugTrackingSystem.Models.Entities;
using System.Text.Json;

namespace BugTrackingSystem.Services.Core
{
    public class ReportingService : IReportingService
    {
        private readonly BugTrackingSystemDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReportingService(BugTrackingSystemDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<BugReportAnalyticsViewModel> GetBugAnalyticsAsync(ReportFilterViewModel filters)
        {
            var query = BuildBaseQuery(filters);
            var bugs = await query.ToListAsync();

            var summary = new ReportSummaryViewModel
            {
                TotalBugs = bugs.Count,
                OpenBugs = bugs.Count(b => b.Status.Name != "Closed"),
                ResolvedBugs = bugs.Count(b => b.Status.Name == "Ready for Regression"),
                ClosedBugs = bugs.Count(b => b.Status.Name == "Closed"),
                HighPriorityBugs = bugs.Count(b => b.Priority.Name == "High"),
                CriticalBugs = bugs.Count(b => b.Priority.Name == "Critical"),
                BugsCreatedInPeriod = bugs.Count(b => IsInDateRange(b.CreatedAt, filters)),
                BugsResolvedInPeriod = bugs.Count(b => b.Status.Name == "Closed" && b.UpdatedAt.HasValue && IsInDateRange(b.UpdatedAt.Value, filters))
            };

            if (summary.TotalBugs > 0)
            {
                summary.ResolutionRate = (double)summary.ClosedBugs / summary.TotalBugs * 100;
                var resolvedBugs = bugs.Where(b => b.Status.Name == "Closed" && b.UpdatedAt.HasValue).ToList();
                if (resolvedBugs.Any())
                {
                    summary.AverageResolutionDays = resolvedBugs.Average(b => (b.UpdatedAt!.Value - b.CreatedAt).TotalDays);
                }
            }

            var statusDistribution = bugs.GroupBy(b => b.Status.Name)
                .Select(g => new StatusDistributionViewModel
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = (double)g.Count() / bugs.Count * 100,
                    Color = GetStatusColor(g.Key)
                }).ToList();

            var priorityDistribution = bugs.GroupBy(b => b.Priority.Name)
                .Select(g => new PriorityDistributionViewModel
                {
                    Priority = g.Key,
                    Count = g.Count(),
                    Percentage = (double)g.Count() / bugs.Count * 100,
                    Color = GetPriorityColor(g.Key)
                }).ToList();

            var applicationDistribution = bugs.GroupBy(b => b.ApplicationName.Name)
                .Select(g => new ApplicationDistributionViewModel
                {
                    Application = g.Key,
                    TotalBugs = g.Count(),
                    OpenBugs = g.Count(b => b.Status.Name != "Closed"),
                    ResolvedBugs = g.Count(b => b.Status.Name == "Closed"),
                    ResolutionRate = g.Count() > 0 ? (double)g.Count(b => b.Status.Name == "Closed") / g.Count() * 100 : 0
                }).ToList();

            var trendData = await GetTrendDataAsync(filters);

            return new BugReportAnalyticsViewModel
            {
                Summary = summary,
                StatusDistribution = statusDistribution,
                PriorityDistribution = priorityDistribution,
                ApplicationDistribution = applicationDistribution,
                TrendData = trendData,
                AppliedFilters = filters,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<BugReportDetailViewModel>> GetDetailedBugReportAsync(ReportFilterViewModel filters)
        {
            var query = BuildBaseQuery(filters);
            
            // Apply sorting
            query = ApplySorting(query, filters);

            // Apply pagination
            var bugs = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return bugs.Select(b => new BugReportDetailViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description.Length > 100 ? b.Description.Substring(0, 100) + "..." : b.Description,
                Status = b.Status.Name,
                Priority = b.Priority.Name,
                Application = b.ApplicationName.Name,
                Reporter = b.Reporter.UserName ?? "Unknown",
                AssignedTo = b.AssignedToUser?.UserName ?? "Unassigned",
                Developer = b.Developer?.UserName ?? "Unassigned",
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                ResolvedAt = b.Status.Name == "Closed" ? b.UpdatedAt : null,
                DaysToResolve = b.Status.Name == "Closed" && b.UpdatedAt.HasValue ? 
                    (int)(b.UpdatedAt.Value - b.CreatedAt).TotalDays : 0,
                CommentCount = b.Comments.Count,
                IsOverdue = (DateTime.UtcNow - b.CreatedAt).TotalDays > 30 && b.Status.Name != "Closed",
                StatusColor = GetStatusColor(b.Status.Name),
                PriorityColor = GetPriorityColor(b.Priority.Name)
            }).ToList();
        }

        public async Task<UserProductivityReportViewModel> GetUserProductivityReportAsync(ReportFilterViewModel filters)
        {
            var users = await _userManager.Users.ToListAsync();
            var bugs = await BuildBaseQuery(filters).ToListAsync();

            var userStats = new List<UserProductivityViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var reportedBugs = bugs.Where(b => b.ReporterId == user.Id).ToList();
                var assignedBugs = bugs.Where(b => b.AssignedToUserId == user.Id).ToList();
                var developedBugs = bugs.Where(b => b.DeveloperId == user.Id).ToList();
                var resolvedBugs = assignedBugs.Where(b => b.Status.Name == "Closed").ToList();

                var avgResolutionDays = resolvedBugs.Any() && resolvedBugs.All(b => b.UpdatedAt.HasValue) ?
                    resolvedBugs.Average(b => (b.UpdatedAt!.Value - b.CreatedAt).TotalDays) : 0;

                var productivityScore = CalculateProductivityScore(reportedBugs.Count, resolvedBugs.Count, avgResolutionDays);

                userStats.Add(new UserProductivityViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? "Unknown",
                    Email = user.Email ?? "Unknown",
                    Role = userRoles.FirstOrDefault() ?? "User",
                    BugsReported = reportedBugs.Count,
                    BugsAssigned = assignedBugs.Count,
                    BugsResolved = resolvedBugs.Count,
                    BugsDeveloped = developedBugs.Count,
                    AverageResolutionDays = avgResolutionDays,
                    HighPriorityBugs = assignedBugs.Count(b => b.Priority.Name == "High"),
                    CriticalBugs = assignedBugs.Count(b => b.Priority.Name == "Critical"),
                    ProductivityScore = productivityScore,
                    LastActivity = bugs.Where(b => b.ReporterId == user.Id || b.AssignedToUserId == user.Id)
                        .Max(b => b.UpdatedAt ?? b.CreatedAt),
                    IsActive = bugs.Any(b => (b.ReporterId == user.Id || b.AssignedToUserId == user.Id) && 
                        (b.UpdatedAt ?? b.CreatedAt) > DateTime.UtcNow.AddDays(-30))
                });
            }

            var summary = new UserProductivitySummaryViewModel
            {
                TotalUsers = userStats.Count,
                ActiveUsers = userStats.Count(u => u.IsActive),
                TopReporter = userStats.OrderByDescending(u => u.BugsReported).FirstOrDefault(),
                TopResolver = userStats.OrderByDescending(u => u.BugsResolved).FirstOrDefault(),
                MostProductiveUser = userStats.OrderByDescending(u => u.ProductivityScore).FirstOrDefault(),
                AverageProductivityScore = userStats.Average(u => u.ProductivityScore)
            };

            return new UserProductivityReportViewModel
            {
                UserStats = userStats.OrderByDescending(u => u.ProductivityScore).ToList(),
                Summary = summary,
                AppliedFilters = filters,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<ApplicationBugReportViewModel> GetApplicationBugReportAsync(ReportFilterViewModel filters)
        {
            var applications = await _context.ApplicationName.Where(a => a.IsActive).ToListAsync();
            var bugs = await BuildBaseQuery(filters).ToListAsync();

            var appStats = new List<ApplicationBugStatsViewModel>();

            foreach (var app in applications)
            {
                var appBugs = bugs.Where(b => b.ApplicationId == app.Id).ToList();
                var resolvedBugs = appBugs.Where(b => b.Status.Name == "Closed").ToList();
                
                var thisMonth = DateTime.UtcNow.AddDays(-30);
                var lastMonth = DateTime.UtcNow.AddDays(-60);
                
                var bugsThisMonth = appBugs.Count(b => b.CreatedAt >= thisMonth);
                var bugsLastMonth = appBugs.Count(b => b.CreatedAt >= lastMonth && b.CreatedAt < thisMonth);

                var trendPercentage = bugsLastMonth > 0 ? 
                    ((double)(bugsThisMonth - bugsLastMonth) / bugsLastMonth) * 100 : 0;

                var resolutionRate = appBugs.Count > 0 ? (double)resolvedBugs.Count / appBugs.Count * 100 : 0;
                var avgResolutionDays = resolvedBugs.Any() && resolvedBugs.All(b => b.UpdatedAt.HasValue) ?
                    resolvedBugs.Average(b => (b.UpdatedAt!.Value - b.CreatedAt).TotalDays) : 0;

                var healthStatus = GetApplicationHealthStatus(resolutionRate, avgResolutionDays, bugsThisMonth);

                appStats.Add(new ApplicationBugStatsViewModel
                {
                    ApplicationId = app.Id,
                    ApplicationName = app.Name,
                    TotalBugs = appBugs.Count,
                    OpenBugs = appBugs.Count(b => b.Status.Name != "Closed"),
                    InProgressBugs = appBugs.Count(b => b.Status.Name == "In Development"),
                    ResolvedBugs = appBugs.Count(b => b.Status.Name == "Ready for Regression"),
                    ClosedBugs = resolvedBugs.Count,
                    HighPriorityBugs = appBugs.Count(b => b.Priority.Name == "High"),
                    CriticalBugs = appBugs.Count(b => b.Priority.Name == "Critical"),
                    ResolutionRate = resolutionRate,
                    AverageResolutionDays = avgResolutionDays,
                    BugsThisMonth = bugsThisMonth,
                    BugsLastMonth = bugsLastMonth,
                    TrendPercentage = trendPercentage,
                    HealthStatus = healthStatus.status,
                    HealthColor = healthStatus.color
                });
            }

            var summary = new ApplicationSummaryViewModel
            {
                TotalApplications = appStats.Count,
                MostBuggyApp = appStats.OrderByDescending(a => a.TotalBugs).FirstOrDefault(),
                BestPerformingApp = appStats.OrderByDescending(a => a.ResolutionRate).FirstOrDefault(),
                MostImprovedApp = appStats.OrderBy(a => a.TrendPercentage).FirstOrDefault(),
                OverallResolutionRate = appStats.Any() ? appStats.Average(a => a.ResolutionRate) : 0
            };

            return new ApplicationBugReportViewModel
            {
                ApplicationStats = appStats,
                Summary = summary,
                AppliedFilters = filters,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<TimeBasedReportViewModel> GetTimeBasedReportAsync(ReportFilterViewModel filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            var timeSeriesData = new List<TimeSeriesDataViewModel>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                var nextDate = currentDate.AddDays(1);
                
                var dayBugs = await _context.BugReports
                    .Include(b => b.Status)
                    .Where(b => b.CreatedAt >= currentDate && b.CreatedAt < nextDate)
                    .ToListAsync();

                var resolvedBugs = await _context.BugReports
                    .Include(b => b.Status)
                    .Where(b => b.Status.Name == "Closed" && 
                               b.UpdatedAt.HasValue && 
                               b.UpdatedAt.Value >= currentDate && 
                               b.UpdatedAt.Value < nextDate)
                    .CountAsync();

                var inProgressBugs = await _context.BugReports
                    .Include(b => b.Status)
                    .Where(b => b.Status.Name == "In Development" && 
                               b.CreatedAt <= currentDate)
                    .CountAsync();

                var created = dayBugs.Count;
                var netChange = created - resolvedBugs;
                var resolutionRate = created > 0 ? (double)resolvedBugs / created * 100 : 0;

                timeSeriesData.Add(new TimeSeriesDataViewModel
                {
                    Date = currentDate,
                    Period = currentDate.ToString("MMM dd"),
                    BugsCreated = created,
                    BugsResolved = resolvedBugs,
                    BugsInProgress = inProgressBugs,
                    NetChange = netChange,
                    ResolutionRate = resolutionRate
                });

                currentDate = nextDate;
            }

            var busiestDayData = timeSeriesData.OrderByDescending(t => t.BugsCreated).FirstOrDefault();
            var mostProductiveDayData = timeSeriesData.OrderByDescending(t => t.BugsResolved).FirstOrDefault();

            var summary = new TimeBasedSummaryViewModel
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalDays = (int)(endDate - startDate).TotalDays,
                AverageBugsPerDay = timeSeriesData.Average(t => t.BugsCreated),
                AverageResolutionPerDay = timeSeriesData.Average(t => t.BugsResolved),
                BusiestDay = busiestDayData != null ? new PeakTimeViewModel
                {
                    Period = busiestDayData.Date.ToString("MMM dd, yyyy"),
                    TimeFrame = "Day",
                    BugCount = busiestDayData.BugsCreated,
                    Type = "Created"
                } : null,
                MostProductiveDay = mostProductiveDayData != null ? new PeakTimeViewModel
                {
                    Period = mostProductiveDayData.Date.ToString("MMM dd, yyyy"),
                    TimeFrame = "Day",
                    BugCount = mostProductiveDayData.BugsResolved,
                    Type = "Resolved"
                } : null
            };

            return new TimeBasedReportViewModel
            {
                TimeSeriesData = timeSeriesData,
                Summary = summary,
                AppliedFilters = filters,
                GeneratedAt = DateTime.UtcNow
            };
        }

        // Helper methods will be implemented in the next part...
        private IQueryable<BugReport> BuildBaseQuery(ReportFilterViewModel filters)
        {
            var query = _context.BugReports
                .Include(b => b.Status)
                .Include(b => b.Priority)
                .Include(b => b.ApplicationName)
                .Include(b => b.Reporter)
                .Include(b => b.AssignedToUser)
                .Include(b => b.Developer)
                .Include(b => b.Comments)
                .Where(b => b.IsActive);

            if (filters.StartDate.HasValue)
                query = query.Where(b => b.CreatedAt >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(b => b.CreatedAt <= filters.EndDate.Value);

            if (filters.StatusIds.Any())
                query = query.Where(b => filters.StatusIds.Contains(b.StatusId));

            if (filters.PriorityIds.Any())
                query = query.Where(b => filters.PriorityIds.Contains(b.PriorityId));

            if (filters.ApplicationIds.Any())
                query = query.Where(b => filters.ApplicationIds.Contains(b.ApplicationId));

            if (filters.AssignedUserIds.Any())
                query = query.Where(b => b.AssignedToUserId != null && filters.AssignedUserIds.Contains(b.AssignedToUserId));

            if (filters.ReporterIds.Any())
                query = query.Where(b => filters.ReporterIds.Contains(b.ReporterId));

            if (filters.DeveloperIds.Any())
                query = query.Where(b => b.DeveloperId != null && filters.DeveloperIds.Contains(b.DeveloperId));

            if (!filters.IncludeResolved)
                query = query.Where(b => b.Status.Name != "Ready for Regression");

            if (!filters.IncludeClosed)
                query = query.Where(b => b.Status.Name != "Closed");

            return query;
        }

        public async Task<byte[]> ExportBugReportToPdfAsync(ReportFilterViewModel filters)
        {
            // For now, return a placeholder - PDF generation would require a library like iTextSharp
            var message = "PDF export functionality will be implemented with a PDF library like iTextSharp or PuppeteerSharp";
            return System.Text.Encoding.UTF8.GetBytes(message);
        }

        public async Task<byte[]> ExportBugReportToExcelAsync(ReportFilterViewModel filters)
        {
            // For now, return a placeholder - Excel generation would require a library like EPPlus
            var message = "Excel export functionality will be implemented with a library like EPPlus or ClosedXML";
            return System.Text.Encoding.UTF8.GetBytes(message);
        }

        public async Task<List<ReportTemplateViewModel>> GetReportTemplatesAsync()
        {
            // Placeholder for report templates - would be stored in database
            return new List<ReportTemplateViewModel>
            {
                new ReportTemplateViewModel
                {
                    Id = 1,
                    Name = "Weekly Summary",
                    Description = "Weekly bug report summary",
                    ReportType = ReportType.Summary,
                    IsDefault = true,
                    IsPublic = true
                }
            };
        }

        public async Task<ReportFilterOptionsViewModel> GetFilterOptionsAsync()
        {
            var statuses = await _context.BugStatuses.ToListAsync();
            var priorities = await _context.BugPriorities.ToListAsync();
            var applications = await _context.ApplicationName.Where(a => a.IsActive).ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            return new ReportFilterOptionsViewModel
            {
                Statuses = statuses.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList(),

                Priorities = priorities.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList(),

                Applications = applications.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList(),

                Users = users.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName ?? u.Email ?? "Unknown"
                }).ToList(),

                Reporters = users.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName ?? u.Email ?? "Unknown"
                }).ToList(),

                Developers = users.Where(u => _userManager.IsInRoleAsync(u, "Developer").Result)
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = u.Id,
                        Text = u.UserName ?? u.Email ?? "Unknown"
                    }).ToList()
            };
        }

        // Helper methods
        private async Task<List<TrendDataViewModel>> GetTrendDataAsync(ReportFilterViewModel filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = filters.EndDate ?? DateTime.UtcNow;
            var trendData = new List<TrendDataViewModel>();

            var currentDate = startDate.Date;
            while (currentDate <= endDate.Date)
            {
                var nextDate = currentDate.AddDays(1);
                
                var created = await _context.BugReports
                    .Where(b => b.CreatedAt >= currentDate && b.CreatedAt < nextDate)
                    .CountAsync();

                var resolved = await _context.BugReports
                    .Include(b => b.Status)
                    .Where(b => b.Status.Name == "Closed" && 
                               b.UpdatedAt.HasValue && 
                               b.UpdatedAt.Value >= currentDate && 
                               b.UpdatedAt.Value < nextDate)
                    .CountAsync();

                var open = await _context.BugReports
                    .Include(b => b.Status)
                    .Where(b => b.Status.Name != "Closed" && b.CreatedAt <= currentDate)
                    .CountAsync();

                trendData.Add(new TrendDataViewModel
                {
                    Date = currentDate,
                    Created = created,
                    Resolved = resolved,
                    Open = open,
                    Period = currentDate.ToString("MMM dd")
                });

                currentDate = nextDate;
            }

            return trendData;
        }

        private IQueryable<BugReport> ApplySorting(IQueryable<BugReport> query, ReportFilterViewModel filters)
        {
            return filters.SortBy switch
            {
                SortOption.CreatedDate => filters.SortDirection == SortDirection.Ascending 
                    ? query.OrderBy(b => b.CreatedAt) 
                    : query.OrderByDescending(b => b.CreatedAt),
                SortOption.UpdatedDate => filters.SortDirection == SortDirection.Ascending 
                    ? query.OrderBy(b => b.UpdatedAt) 
                    : query.OrderByDescending(b => b.UpdatedAt),
                SortOption.Priority => filters.SortDirection == SortDirection.Ascending 
                    ? query.OrderBy(b => b.Priority.Name) 
                    : query.OrderByDescending(b => b.Priority.Name),
                SortOption.Status => filters.SortDirection == SortDirection.Ascending 
                    ? query.OrderBy(b => b.Status.Name) 
                    : query.OrderByDescending(b => b.Status.Name),
                SortOption.Title => filters.SortDirection == SortDirection.Ascending 
                    ? query.OrderBy(b => b.Title) 
                    : query.OrderByDescending(b => b.Title),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };
        }

        private bool IsInDateRange(DateTime date, ReportFilterViewModel filters)
        {
            if (filters.StartDate.HasValue && date < filters.StartDate.Value)
                return false;
            if (filters.EndDate.HasValue && date > filters.EndDate.Value)
                return false;
            return true;
        }

        private string GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "new" => "#17a2b8",
                "in test" => "#ffc107",
                "in development" => "#007bff",
                "ready for regression" => "#17a2b8",
                "closed" => "#28a745",
                _ => "#6c757d"
            };
        }

        private string GetPriorityColor(string priority)
        {
            return priority?.ToLower() switch
            {
                "critical" => "#dc3545",
                "high" => "#fd7e14",
                "medium" => "#17a2b8",
                "low" => "#6c757d",
                _ => "#6c757d"
            };
        }

        private double CalculateProductivityScore(int reported, int resolved, double avgResolutionDays)
        {
            // Simple productivity scoring algorithm
            var baseScore = resolved * 10;
            var reportingBonus = reported * 2;
            var speedBonus = avgResolutionDays > 0 ? Math.Max(0, 100 - avgResolutionDays * 2) : 0;
            
            return Math.Max(0, baseScore + reportingBonus + speedBonus);
        }

        private (string status, string color) GetApplicationHealthStatus(double resolutionRate, double avgResolutionDays, int bugsThisMonth)
        {
            if (resolutionRate >= 80 && avgResolutionDays <= 7 && bugsThisMonth <= 10)
                return ("Excellent", "#28a745");
            else if (resolutionRate >= 60 && avgResolutionDays <= 14 && bugsThisMonth <= 20)
                return ("Good", "#17a2b8");
            else if (resolutionRate >= 40 && avgResolutionDays <= 21 && bugsThisMonth <= 30)
                return ("Fair", "#ffc107");
            else
                return ("Needs Attention", "#dc3545");
        }
    }
}