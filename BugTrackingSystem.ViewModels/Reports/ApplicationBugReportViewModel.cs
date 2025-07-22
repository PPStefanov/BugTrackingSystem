namespace BugTrackingSystem.ViewModels.Reports
{
    public class ApplicationBugReportViewModel
    {
        public List<ApplicationBugStatsViewModel> ApplicationStats { get; set; }
        public ApplicationSummaryViewModel Summary { get; set; }
        public ReportFilterViewModel AppliedFilters { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class ApplicationBugStatsViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int TotalBugs { get; set; }
        public int OpenBugs { get; set; }
        public int InProgressBugs { get; set; }
        public int ResolvedBugs { get; set; }
        public int ClosedBugs { get; set; }
        public int HighPriorityBugs { get; set; }
        public int CriticalBugs { get; set; }
        public double ResolutionRate { get; set; }
        public double AverageResolutionDays { get; set; }
        public int BugsThisMonth { get; set; }
        public int BugsLastMonth { get; set; }
        public double TrendPercentage { get; set; }
        public string HealthStatus { get; set; }
        public string HealthColor { get; set; }
    }

    public class ApplicationSummaryViewModel
    {
        public int TotalApplications { get; set; }
        public ApplicationBugStatsViewModel MostBuggyApp { get; set; }
        public ApplicationBugStatsViewModel BestPerformingApp { get; set; }
        public ApplicationBugStatsViewModel MostImprovedApp { get; set; }
        public double OverallResolutionRate { get; set; }
    }
}