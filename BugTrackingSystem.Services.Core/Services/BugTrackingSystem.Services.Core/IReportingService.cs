using BugTrackingSystem.ViewModels.Reports;

namespace BugTrackingSystem.Services.Core
{
    public interface IReportingService
    {
        Task<BugReportAnalyticsViewModel> GetBugAnalyticsAsync(ReportFilterViewModel filters);
        Task<List<BugReportDetailViewModel>> GetDetailedBugReportAsync(ReportFilterViewModel filters);
        Task<UserProductivityReportViewModel> GetUserProductivityReportAsync(ReportFilterViewModel filters);
        Task<ApplicationBugReportViewModel> GetApplicationBugReportAsync(ReportFilterViewModel filters);
        Task<TimeBasedReportViewModel> GetTimeBasedReportAsync(ReportFilterViewModel filters);
        Task<byte[]> ExportBugReportToPdfAsync(ReportFilterViewModel filters);
        Task<byte[]> ExportBugReportToExcelAsync(ReportFilterViewModel filters);
        Task<List<ReportTemplateViewModel>> GetReportTemplatesAsync();
        Task<ReportFilterOptionsViewModel> GetFilterOptionsAsync();
    }
}