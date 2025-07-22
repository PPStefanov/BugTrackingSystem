namespace BugTrackingSystem.ViewModels.Reports
{
    public class UserProductivityReportViewModel
    {
        public List<UserProductivityViewModel> UserStats { get; set; }
        public UserProductivitySummaryViewModel Summary { get; set; }
        public ReportFilterViewModel AppliedFilters { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserProductivityViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int BugsReported { get; set; }
        public int BugsAssigned { get; set; }
        public int BugsResolved { get; set; }
        public int BugsDeveloped { get; set; }
        public double AverageResolutionDays { get; set; }
        public int HighPriorityBugs { get; set; }
        public int CriticalBugs { get; set; }
        public double ProductivityScore { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserProductivitySummaryViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public UserProductivityViewModel TopReporter { get; set; }
        public UserProductivityViewModel TopResolver { get; set; }
        public UserProductivityViewModel MostProductiveUser { get; set; }
        public double AverageProductivityScore { get; set; }
    }
}