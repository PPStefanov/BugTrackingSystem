using BugTrackingSystem.Services.Core;
using BugTrackingSystem.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackingSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportingService _reportingService;
        private readonly IBugReportService _bugReportService;

        public ReportsController(IReportingService reportingService, IBugReportService bugReportService)
        {
            _reportingService = reportingService;
            _bugReportService = bugReportService;
        }

        public async Task<IActionResult> Index()
        {
            var filterOptions = await _reportingService.GetFilterOptionsAsync();
            var defaultFilters = new ReportFilterViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                ReportType = ReportType.Summary,
                IncludeResolved = true,
                IncludeClosed = true
            };

            ViewBag.FilterOptions = filterOptions;
            return View(defaultFilters);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(ReportFilterViewModel filters)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = await _reportingService.GetFilterOptionsAsync();
                ViewBag.FilterOptions = filterOptions;
                return View("Index", filters);
            }

            try
            {
                switch (filters.ReportType)
                {
                    case ReportType.Summary:
                        var analytics = await _reportingService.GetBugAnalyticsAsync(filters);
                        return View("Analytics", analytics);

                    case ReportType.Detailed:
                        var detailedReport = await _reportingService.GetDetailedBugReportAsync(filters);
                        ViewBag.Filters = filters;
                        return View("DetailedReport", detailedReport);

                    case ReportType.UserProductivity:
                        var userReport = await _reportingService.GetUserProductivityReportAsync(filters);
                        return View("UserProductivity", userReport);

                    case ReportType.ApplicationAnalysis:
                        var appReport = await _reportingService.GetApplicationBugReportAsync(filters);
                        return View("ApplicationAnalysis", appReport);

                    case ReportType.TimeBased:
                        var timeReport = await _reportingService.GetTimeBasedReportAsync(filters);
                        return View("TimeBased", timeReport);

                    default:
                        var defaultAnalytics = await _reportingService.GetBugAnalyticsAsync(filters);
                        return View("Analytics", defaultAnalytics);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
                var filterOptions = await _reportingService.GetFilterOptionsAsync();
                ViewBag.FilterOptions = filterOptions;
                return View("Index", filters);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf(ReportFilterViewModel filters)
        {
            try
            {
                var pdfBytes = await _reportingService.ExportBugReportToPdfAsync(filters);
                var fileName = $"BugReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error exporting to PDF: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(ReportFilterViewModel filters)
        {
            try
            {
                var excelBytes = await _reportingService.ExportBugReportToExcelAsync(filters);
                var fileName = $"BugReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error exporting to Excel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuickReport(string type)
        {
            var filters = new ReportFilterViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                IncludeResolved = true,
                IncludeClosed = true
            };

            switch (type?.ToLower())
            {
                case "weekly":
                    filters.StartDate = DateTime.Now.AddDays(-7);
                    filters.ReportType = ReportType.Summary;
                    break;
                case "monthly":
                    filters.StartDate = DateTime.Now.AddDays(-30);
                    filters.ReportType = ReportType.Summary;
                    break;
                case "quarterly":
                    filters.StartDate = DateTime.Now.AddDays(-90);
                    filters.ReportType = ReportType.Summary;
                    break;
                case "users":
                    filters.ReportType = ReportType.UserProductivity;
                    break;
                case "applications":
                    filters.ReportType = ReportType.ApplicationAnalysis;
                    break;
                default:
                    filters.ReportType = ReportType.Summary;
                    break;
            }

            return await GenerateReport(filters);
        }

        [HttpGet]
        public async Task<JsonResult> GetFilterData(string type)
        {
            try
            {
                switch (type?.ToLower())
                {
                    case "statuses":
                        var statuses = await _bugReportService.GetBugStatusesAsync();
                        return Json(statuses.Select(s => new { id = s.Id, name = s.Name }));

                    case "priorities":
                        var priorities = await _bugReportService.GetBugPrioritiesAsync();
                        return Json(priorities.Select(p => new { id = p.Id, name = p.Name }));

                    case "applications":
                        var applications = await _bugReportService.GetApplicationsAsync();
                        return Json(applications.Select(a => new { id = a.Id, name = a.Name }));

                    default:
                        return Json(new { error = "Invalid filter type" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}