namespace BugTrackingSystem.ViewModels.Reports
{
    public class BugReportAnalyticsViewModel
    {
        public ReportSummaryViewModel Summary { get; set; }
        public List<StatusDistributionViewModel> StatusDistribution { get; set; }
        public List<PriorityDistributionViewModel> PriorityDistribution { get; set; }
        public List<ApplicationDistributionViewModel> ApplicationDistribution { get; set; }
        public List<UserProductivityViewModel> TopReporters { get; set; }
        public List<UserProductivityViewModel> TopResolvers { get; set; }
        public List<TrendDataViewModel> TrendData { get; set; }
        public List<AverageResolutionTimeViewModel> ResolutionTimes { get; set; }
        public ReportFilterViewModel AppliedFilters { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class ReportSummaryViewModel
    {
        public int TotalBugs { get; set; }
        public int OpenBugs { get; set; }
        public int ResolvedBugs { get; set; }
        public int ClosedBugs { get; set; }
        public int HighPriorityBugs { get; set; }
        public int CriticalBugs { get; set; }
        public double AverageResolutionDays { get; set; }
        public int BugsCreatedInPeriod { get; set; }
        public int BugsResolvedInPeriod { get; set; }
        public double ResolutionRate { get; set; }
    }

    public class StatusDistributionViewModel
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; }
    }

    public class PriorityDistributionViewModel
    {
        public string Priority { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; }
    }

    public class ApplicationDistributionViewModel
    {
        public string Application { get; set; }
        public int TotalBugs { get; set; }
        public int OpenBugs { get; set; }
        public int ResolvedBugs { get; set; }
        public double ResolutionRate { get; set; }
    }

    public class TrendDataViewModel
    {
        public DateTime Date { get; set; }
        public int Created { get; set; }
        public int Resolved { get; set; }
        public int Open { get; set; }
        public string Period { get; set; }
    }

    public class AverageResolutionTimeViewModel
    {
        public string Category { get; set; }
        public double AverageDays { get; set; }
        public int Count { get; set; }
    }
}