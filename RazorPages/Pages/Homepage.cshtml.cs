using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels;

namespace BugTrackingSystem.Pages
{
    public class HomepageModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public HomepageViewModel ViewModel { get; set; }

        public HomepageModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
            ViewModel = new HomepageViewModel
            {
                WelcomeMessage = "Welcome to the Bug Tracking System!",
                IsUserLoggedIn = _signInManager.IsSignedIn(User)
            };
        }
    }
}