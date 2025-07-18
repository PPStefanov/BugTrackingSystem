namespace BugTrackingSystem.ViewModels.Dashboard
{
    public class RecentActivityViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string ActivityType { get; set; } // Created, Updated, Resolved, Assigned, etc.
    }
}