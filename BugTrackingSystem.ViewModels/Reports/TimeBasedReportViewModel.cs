namespace BugTrackingSystem.ViewModels.Reports
{
    public class TimeBasedReportViewModel
    {
        public List<TimeSeriesDataViewModel> TimeSeriesData { get; set; }
        public List<PeakTimeViewModel> PeakTimes { get; set; }
        public TimeBasedSummaryViewModel Summary { get; set; }
        public ReportFilterViewModel AppliedFilters { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class TimeSeriesDataViewModel
    {
        public DateTime Date { get; set; }
        public string Period { get; set; }
        public int BugsCreated { get; set; }
        public int BugsResolved { get; set; }
        public int BugsInProgress { get; set; }
        public int NetChange { get; set; }
        public double ResolutionRate { get; set; }
    }

    public class PeakTimeViewModel
    {
        public string Period { get; set; }
        public string TimeFrame { get; set; }
        public int BugCount { get; set; }
        public string Type { get; set; } // Created, Resolved, etc.
    }

    public class TimeBasedSummaryViewModel
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalDays { get; set; }
        public double AverageBugsPerDay { get; set; }
        public double AverageResolutionPerDay { get; set; }
        public PeakTimeViewModel BusiestDay { get; set; }
        public PeakTimeViewModel MostProductiveDay { get; set; }
        public string TrendDirection { get; set; }
        public double TrendPercentage { get; set; }
    }
}