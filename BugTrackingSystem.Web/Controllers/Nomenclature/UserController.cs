using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackingSystem.Web.Controllers.Nomenclature
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // List all users
        public async Task<IActionResult> List()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "",
                    IsActive = true // You can add logic for IsActive if needed
                });
            }
            return View(userViewModels);
        }

        // Show create user form
        [HttpGet]
        public IActionResult Create()
        {
            var model = new UserEditViewModel
            {
                RoleList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            };
            return View(model);
        }

        // Handle create user POST
        [HttpPost]
        public async Task<IActionResult> Create(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RoleList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.Role))
                    await _userManager.AddToRoleAsync(user, model.Role);
                return RedirectToAction("List");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            model.RoleList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
            return View(model);
        }

        // Show edit user form
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                RoleList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            };
            return View(model);
        }

        // Handle edit user POST
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RoleList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                model.RoleList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View(model);
            }

            // Update roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!string.IsNullOrEmpty(model.Role) && !currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return RedirectToAction("List");
        }

        // Delete user
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("List");
        }
    }
}