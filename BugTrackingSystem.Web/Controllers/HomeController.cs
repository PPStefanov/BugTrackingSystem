using Microsoft.AspNetCore.Mvc;
using BugTrackingSystem.ViewModels;

namespace BugTrackingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new HomepageViewModel
            {
                WelcomeMessage = "Welcome to the Bug Tracking System!",
                IsUserLoggedIn = User.Identity.IsAuthenticated
            };

            return View(viewModel);
        }
    }
}   