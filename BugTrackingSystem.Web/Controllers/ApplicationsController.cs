using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        // GET: Applications
        public async Task<IActionResult> List()
        {
            var applications = await _applicationService.GetAllApplicationsAsync();
            return View(applications);
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _applicationService.GetApplicationWithBugReportsAsync(id.Value);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] ApplicationName application)
        {
            if (ModelState.IsValid)
            {
                var createdApp = await _applicationService.CreateApplicationAsync(application);
                TempData["SuccessMessage"] = $"Application '{createdApp.Name}' has been created successfully.";
                return RedirectToAction(nameof(List));
            }
            return View(application);
        }

        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _applicationService.GetApplicationByIdAsync(id.Value);
            if (application == null)
            {
                return NotFound();
            }
            return View(application);
        }

        // POST: Applications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsActive")] ApplicationName application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedApp = await _applicationService.UpdateApplicationAsync(application);
                if (updatedApp == null)
                {
                    return NotFound();
                }
                
                TempData["SuccessMessage"] = $"Application '{updatedApp.Name}' has been updated successfully.";
                return RedirectToAction(nameof(List));
            }
            return View(application);
        }

        // GET: Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _applicationService.GetApplicationWithBugReportsAsync(id.Value);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _applicationService.GetApplicationWithBugReportsAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            var deleted = await _applicationService.DeleteApplicationAsync(id);
            if (deleted)
            {
                TempData["SuccessMessage"] = $"Application '{application.Name}' has been deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Cannot delete application '{application.Name}' because it has associated bug reports. Please deactivate it instead.";
            }

            return RedirectToAction(nameof(List));
        }

        // POST: Applications/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            var toggled = await _applicationService.ToggleApplicationStatusAsync(id);
            if (toggled)
            {
                var status = !application.IsActive ? "activated" : "deactivated";
                TempData["SuccessMessage"] = $"Application '{application.Name}' has been {status} successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update application status.";
            }

            return RedirectToAction(nameof(List));
        }

        private async Task<bool> ApplicationExists(int id)
        {
            return await _applicationService.ApplicationExistsAsync(id);
        }
    }
}