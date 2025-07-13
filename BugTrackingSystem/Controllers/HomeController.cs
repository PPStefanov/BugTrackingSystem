using System.Diagnostics;
using BugTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using BugTrackingSystem.ViewModels;


namespace BugTrackingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                WelcomeMessage = "Welcome to the Bug Tracking System!",
                IsUserLoggedIn = User.Identity.IsAuthenticated
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
