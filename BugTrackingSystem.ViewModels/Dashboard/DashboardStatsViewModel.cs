namespace BugTrackingSystem.ViewModels.Dashboard
{
    public class DashboardStatsViewModel
    {
        public int TotalBugReports { get; set; }
        public int ResolvedIssues { get; set; }
        public int PendingReview { get; set; }
        public int ActiveUsers { get; set; }
        public int NewThisWeek { get; set; }
        public int InProgress { get; set; }
        public int HighPriority { get; set; }
        public int OverdueIssues { get; set; }
        
        // Percentage changes from last period
        public double TotalBugReportsChange { get; set; }
        public double ResolvedIssuesChange { get; set; }
        public double PendingReviewChange { get; set; }
        public double ActiveUsersChange { get; set; }
    }
}