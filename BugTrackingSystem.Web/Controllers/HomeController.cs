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
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Return a simple view model for guest users
                var guestViewModel = new DashboardViewModel
                {
                    WelcomeMessage = "Welcome to the Bug Tracking System!",
                    IsUserLoggedIn = false,
                    Stats = null,
                    RecentActivity = new List<RecentActivityViewModel>(),
                    BugsByStatus = new Dictionary<string, int>(),
                    BugsByPriority = new Dictionary<string, int>(),
                    MonthlyStats = new List<MonthlyStatsViewModel>()
                };
                
                return View(guestViewModel);
            }

            // Load dashboard data for authenticated users
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