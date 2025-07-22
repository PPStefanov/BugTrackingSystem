using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.ViewModels.Reports
{
    public class ReportFilterViewModel
    {
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Status")]
        public List<int> StatusIds { get; set; } = new List<int>();

        [Display(Name = "Priority")]
        public List<int> PriorityIds { get; set; } = new List<int>();

        [Display(Name = "Application")]
        public List<int> ApplicationIds { get; set; } = new List<int>();

        [Display(Name = "Assigned To")]
        public List<string> AssignedUserIds { get; set; } = new List<string>();

        [Display(Name = "Reporter")]
        public List<string> ReporterIds { get; set; } = new List<string>();

        [Display(Name = "Developer")]
        public List<string> DeveloperIds { get; set; } = new List<string>();

        [Display(Name = "Report Type")]
        public ReportType ReportType { get; set; } = ReportType.Summary;

        [Display(Name = "Group By")]
        public GroupByOption GroupBy { get; set; } = GroupByOption.Status;

        [Display(Name = "Include Resolved")]
        public bool IncludeResolved { get; set; } = true;

        [Display(Name = "Include Closed")]
        public bool IncludeClosed { get; set; } = false;

        [Display(Name = "Sort By")]
        public SortOption SortBy { get; set; } = SortOption.CreatedDate;

        [Display(Name = "Sort Direction")]
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;

        public int PageSize { get; set; } = 50;
        public int PageNumber { get; set; } = 1;
    }

    public enum ReportType
    {
        Summary,
        Detailed,
        UserProductivity,
        ApplicationAnalysis,
        TimeBased
    }

    public enum GroupByOption
    {
        Status,
        Priority,
        Application,
        AssignedUser,
        Reporter,
        Developer,
        CreatedDate,
        ResolvedDate
    }

    public enum SortOption
    {
        CreatedDate,
        UpdatedDate,
        Priority,
        Status,
        Title,
        Reporter,
        AssignedUser
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}