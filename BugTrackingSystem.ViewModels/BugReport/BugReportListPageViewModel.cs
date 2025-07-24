namespace BugTrackingSystem.ViewModels.BugReport
{
    public class BugReportListPageViewModel
    {
        public List<BugReportListViewModel> BugReports { get; set; } = new List<BugReportListViewModel>();
        public BugReportFilterViewModel Filters { get; set; } = new BugReportFilterViewModel();
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public bool HasFiltersApplied { get; set; }
    }
}