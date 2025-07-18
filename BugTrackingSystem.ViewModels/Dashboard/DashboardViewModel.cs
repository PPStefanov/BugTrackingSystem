namespace BugTrackingSystem.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public string WelcomeMessage { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public DashboardStatsViewModel Stats { get; set; }
        public List<RecentActivityViewModel> RecentActivity { get; set; }
        public Dictionary<string, int> BugsByStatus { get; set; }
        public Dictionary<string, int> BugsByPriority { get; set; }
        public List<MonthlyStatsViewModel> MonthlyStats { get; set; }
    }
}