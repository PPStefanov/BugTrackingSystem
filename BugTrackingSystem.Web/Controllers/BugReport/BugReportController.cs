using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Models.Enums;
using BugTrackingSystem.ViewModels.BugReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using BugReportEntity = BugTrackingSystem.Models.Entities.BugReport;

namespace BugTrackingSystem.Web.Controllers.BugReport
{
    [Authorize]
    public class BugReportController : Controller
    {
        private readonly IBugReportService _bugReportService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BugReportController(IBugReportService bugReportService, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _bugReportService = bugReportService;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> List()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var userId = user.Id;
            var priorities = await _bugReportService.GetBugPrioritiesAsync();

            List<BugReportEntity> tickets;

            if (userRole == "Admin" || userRole == "QA")
            {
                tickets = (await _bugReportService.GetAllBugsAsync())
                    .Where(t => t.Status?.Name != "Closed")
                    .ToList();
            }
            else if (userRole == "Developer")
            {
                tickets = (await _bugReportService.GetBugsByStatusAsync("In Development"))
                    .Where(t => t.Status?.Name != "Closed")
                    .ToList();
            }
            else
            {
                tickets = (await _bugReportService.GetBugsForUserAsync(user))
                    .Where(t => t.Status?.Name != "Closed")
                    .ToList();
            }

            var viewModel = BuildBugReportListViewModel(tickets, priorities, userRole, userId);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction("Form");
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            var priorities = await _bugReportService.GetBugPrioritiesAsync();
            var applications = await _bugReportService.GetApplicationsAsync();
            var statuses = await _bugReportService.GetBugStatusesAsync();
            var users = await GetAssignableUsersAsync();
            var developers = await GetDevelopersAsync();

            var user = await _userManager.GetUserAsync(User);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (id == null)
            {
                var newStatus = statuses.FirstOrDefault(s => s.Name == "New");

                var viewModel = new BugReportFormViewModel
                {
                    StatusId = newStatus?.Id ?? 0,
                    Priorities = priorities,
                    Applications = applications,
                    Statuses = statuses,
                    Users = users,
                    Developers = developers,
                    CanEditStatus = true,
                    ShowDeveloperDropdown = false,
                    IsAdmin = userRole == "Admin"
                };
                return View(viewModel);
            }
            else
            {
                var bugReport = await _bugReportService.GetByIdAsync(id.Value);
                if (bugReport == null) return NotFound();

                // Auto-advance status to "In Test" if QA opens a "New" ticket
                if (userRole == "QA" && bugReport.Status?.Name == "New")
                {
                    var statusChanged = await _bugReportService.AutoAdvanceStatusToInTestIfQAAsync(bugReport, user, statuses);
                    if (statusChanged)
                    {
                        bugReport = await _bugReportService.GetByIdAsync(id.Value);
                    }
                }

                if (!users.Any(u => u.Id == bugReport.AssignedToUserId) && bugReport.AssignedToUserId != null)
                {
                    var assignedUser = await _userManager.FindByIdAsync(bugReport.AssignedToUserId);
                    if (assignedUser != null)
                        users.Add(assignedUser);
                }

                // Only allow valid status transitions
                var allowedStatuses = GetAllowedStatusTransitions(bugReport.Status, statuses);

                // QA cannot edit status if it's "In Development"
                bool canEditStatus = !(userRole == "QA" && bugReport.Status?.Name == "In Development");

                var viewModel = new BugReportFormViewModel
                {
                    Id = bugReport.Id,
                    Title = bugReport.Title,
                    Description = bugReport.Description,
                    PriorityId = bugReport.PriorityId,
                    ApplicationId = bugReport.ApplicationId,
                    StatusId = bugReport.StatusId,
                    AssignedToUserId = bugReport.AssignedToUserId,
                    DeveloperId = bugReport.DeveloperId,
                    Priorities = priorities,
                    Applications = applications,
                    Statuses = allowedStatuses,
                    Users = users,
                    Developers = developers,
                    CanEditStatus = canEditStatus,
                    ShowDeveloperDropdown = bugReport.Status?.Name == "In Development" || 
                                          (userRole == "QA" && allowedStatuses.Any(s => s.Name == "In Development")),
                    IsAdmin = userRole == "Admin"
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Form(BugReportCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new BugReportFormViewModel
                {
                    Title = model.Title,
                    Description = model.Description,
                    PriorityId = model.PriorityId,
                    ApplicationId = model.ApplicationId,
                    StatusId = model.StatusId,
                    AssignedToUserId = model.AssignedToUserId,
                    Priorities = await _bugReportService.GetBugPrioritiesAsync(),
                    Applications = await _bugReportService.GetApplicationsAsync(),
                    Statuses = await _bugReportService.GetBugStatusesAsync(),
                    Users = await GetAssignableUsersAsync()
                };
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            if (model.Id == 0)
            {
                var bugReport = new BugReportEntity
                {
                    Title = model.Title,
                    Description = model.Description,
                    PriorityId = model.PriorityId,
                    ApplicationId = model.ApplicationId,
                    StatusId = model.StatusId,
                    ReporterId = user.Id,
                    AssignedToUserId = model.AssignedToUserId
                };

                await _bugReportService.CreateAsync(bugReport, user);

                await _emailSender.SendEmailAsync(
                    "qa-team@example.com",
                    "New Bug Report Created",
                    $"A new bug report '{bugReport.Title}' has been created."
                );

                TempData["SuccessMessage"] = "Bug report created successfully!";
            }
            else
            {
                var bugReport = await _bugReportService.GetByIdAsync(model.Id);
                if (bugReport == null) return NotFound();

                bugReport.Title = model.Title;
                bugReport.Description = model.Description;
                bugReport.PriorityId = model.PriorityId;
                bugReport.ApplicationId = model.ApplicationId;
                bugReport.StatusId = model.StatusId;
                bugReport.AssignedToUserId = model.AssignedToUserId;
                
                // Handle developer assignment when status is "In Development"
                var status = await _bugReportService.GetBugStatusByIdAsync(model.StatusId);
                if (status?.Name == "In Development" && !string.IsNullOrEmpty(model.DeveloperId))
                {
                    bugReport.DeveloperId = model.DeveloperId;
                }

                await _bugReportService.UpdateAsync(bugReport, user);

                TempData["SuccessMessage"] = "Bug report updated successfully!";
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bugReport = await _bugReportService.GetByIdAsync(id);
            if (bugReport == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var statuses = await _bugReportService.GetBugStatusesAsync();

            // Auto-advance status to "In Test" if QA opens a "New" ticket
            if (userRole == "QA" && bugReport.Status?.Name == "New")
            {
                await _bugReportService.AutoAdvanceStatusToInTestIfQAAsync(bugReport, user, statuses);
                bugReport = await _bugReportService.GetByIdAsync(id);
            }

            return View(bugReport);
        }

        private async Task<List<AppUser>> GetAssignableUsersAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (userRole == "Admin")
            {
                var qaUsers = await _userManager.GetUsersInRoleAsync("QA");
                var devUsers = await _userManager.GetUsersInRoleAsync("Developer");
                return qaUsers.Concat(devUsers).ToList();
            }
            else if (userRole == "QA")
            {
                var devUsers = await _userManager.GetUsersInRoleAsync("Developer");
                return devUsers.ToList();
            }
            else if (userRole == "User")
            {
                var qaUsers = await _userManager.GetUsersInRoleAsync("QA");
                return qaUsers.ToList();
            }

            return new List<AppUser>();
        }

        private async Task<List<AppUser>> GetDevelopersAsync()
        {
            return (await _userManager.GetUsersInRoleAsync("Developer")).ToList();
        }

        private List<BugReportListViewModel> BuildBugReportListViewModel(
            IEnumerable<BugReportEntity> tickets,
            IEnumerable<BugPriorityEntity> priorities,
            string userRole,
            string userId)
        {
            return tickets.Select(ticket => new BugReportListViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status?.Name,
                Priority = priorities.FirstOrDefault(p => p.Id == ticket.PriorityId)?.Name ?? "",
                Application = ticket.ApplicationName?.Name,
                AssignedTo = ticket.AssignedToUser?.UserName ?? "Unassigned",
                Reporter = ticket.Reporter?.UserName ?? "Unknown",
                CanEdit =
                    userRole == "Admin" ||
                    (userRole == "User" && ticket.ReporterId == userId && ticket.Status?.Name == "New") ||
                    (userRole == "QA" && ticket.Status?.Name != "In Development") ||
                    (userRole == "Developer" && ticket.Status?.Name == "In Development")
            }).ToList();
        }

        private List<BugStatusEntity> GetAllowedStatusTransitions(BugStatusEntity currentStatus, List<BugStatusEntity> allStatuses)
        {
            if (currentStatus == null)
                return allStatuses;

            var name = currentStatus.Name;
            var allowed = new List<BugStatusEntity>();

            if (name == "In Test")
            {
                allowed = allStatuses.Where(s => s.Name == "In Test" || s.Name == "In Development").ToList();
            }
            else if (name == "In Development")
            {
                allowed = allStatuses.Where(s => s.Name == "In Development" || s.Name == "Ready for Regression").ToList();
            }
            else if (name == "Ready for Regression")
            {
                allowed = allStatuses.Where(s => s.Name == "Ready for Regression" || s.Name == "Closed" || s.Name == "In Development").ToList();
            }
            else
            {
                allowed = allStatuses.Where(s => s.Name == name).ToList();
            }

            return allowed;
        }
    }
}