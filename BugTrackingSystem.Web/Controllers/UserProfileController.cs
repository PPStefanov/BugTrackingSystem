using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BugTrackingSystem.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UserProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "User"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Validate only password-related fields
            ModelState.Remove("UserName");
            ModelState.Remove("Email");
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
            {
                // Reload user data for the view
                var roles = await _userManager.GetRolesAsync(user);
                model.Id = user.Id;
                model.UserName = user.UserName;
                model.Email = user.Email;
                model.Role = roles.FirstOrDefault() ?? "User";
                return View("Index", model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // Reload user data for the view
            var userRoles = await _userManager.GetRolesAsync(user);
            model.Id = user.Id;
            model.UserName = user.UserName;
            model.Email = user.Email;
            model.Role = userRoles.FirstOrDefault() ?? "User";
            
            return View("Index", model);
        }
    }
}