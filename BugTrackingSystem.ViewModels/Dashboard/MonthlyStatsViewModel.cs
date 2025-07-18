namespace BugTrackingSystem.ViewModels.Dashboard
{
    public class MonthlyStatsViewModel
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int Created { get; set; }
        public int Resolved { get; set; }
        public int InProgress { get; set; }
        public string MonthName => $"{Month} {Year}";
    }
}