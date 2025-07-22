namespace BugTrackingSystem.ViewModels.Reports
{
    public class BugReportDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Application { get; set; }
        public string Reporter { get; set; }
        public string AssignedTo { get; set; }
        public string Developer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int DaysToResolve { get; set; }
        public int CommentCount { get; set; }
        public bool IsOverdue { get; set; }
        public string StatusColor { get; set; }
        public string PriorityColor { get; set; }
    }
}