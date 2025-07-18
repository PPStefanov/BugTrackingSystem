using System.Diagnostics;
using BugTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using BugTrackingSystem.ViewModels;
using BugTrackingSystem.Services.Core;
using BugTrackingSystem.ViewModels.Dashboard;

namespace BugTrackingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboardService;

        public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardStats = await _dashboardService.GetDashboardStatsAsync();
            var recentActivity = await _dashboardService.GetRecentActivityAsync(5);
            var bugsByStatus = await _dashboardService.GetBugsByStatusAsync();
            var bugsByPriority = await _dashboardService.GetBugsByPriorityAsync();
            var monthlyStats = await _dashboardService.GetMonthlyStatsAsync(6);

            var viewModel = new DashboardViewModel
            {
                WelcomeMessage = "Welcome to the Bug Tracking System!",
                IsUserLoggedIn = User.Identity.IsAuthenticated,
                Stats = dashboardStats,
                RecentActivity = recentActivity,
                BugsByStatus = bugsByStatus,
                BugsByPriority = bugsByPriority,
                MonthlyStats = monthlyStats
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}   