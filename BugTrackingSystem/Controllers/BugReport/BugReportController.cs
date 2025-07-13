using BugTrackingSystem.Models.Entities;    
using BugTrackingSystem.Models.Enums;
using BugTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


public class BugReportController : Controller
{
    private readonly IBugReportService _bugReportService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender; // Add this field


    public BugReportController(IBugReportService bugReportService, UserManager<AppUser> userManager, IEmailSender emailSender)
    {
        _bugReportService = bugReportService;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var bugs = await _bugReportService.GetBugsForUserAsync(user);
        return View(bugs);
    }

    [Authorize]
    public async Task<IActionResult> List()
    {
        var user = await _userManager.GetUserAsync(User);
        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        List<BugReport> tickets;

        if (userRole == "Admin" || userRole == "QA")
        {
            // QA/Admin can see all tickets
            tickets = await _bugReportService.GetAllBugsAsync();
        }
        else if (userRole == "Developer")
        {
            // Developer can see only tickets with status "In Development"
            tickets = await _bugReportService.GetBugsByStatusAsync("In Development");
        }
        else
        {
            // Regular user can see only their own tickets
            tickets = await _bugReportService.GetBugsForUserAsync(user);
        }

        var viewModel = tickets.Select(ticket => new BugReportListViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Status = ticket.Status.Name, // Use the Name property of BugStatusEntity
            Priority = ticket.Priority.ToString(), // Convert the BugPriority enum to a string
            Application = ticket.ApplicationName.Name,
            AssignedTo = ticket.AssignedToUser?.UserName ?? "Unassigned"
        }).ToList();

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
        var users = await GetAssignableUsersAsync(); // Filter assignable users

        if (id == null)
        {
            // Create operation
            var viewModel = new BugReportFormViewModel
            {
                Priorities = priorities,
                Applications = applications,
                Statuses = statuses,
                Users = users
            };
            return View(viewModel);
        }
        else
        {
            // Edit operation
            var bugReport = await _bugReportService.GetByIdAsync(id.Value);
            if (bugReport == null) return NotFound();

            var viewModel = new BugReportFormViewModel
            {
                Id = bugReport.Id,
                Title = bugReport.Title,
                Description = bugReport.Description,
                PriorityId = bugReport.PriorityId,
                ApplicationId = bugReport.ApplicationId,
                StatusId = bugReport.StatusId,
                AssignedToUserId = bugReport.AssignedToUserId,
                Priorities = priorities,
                Applications = applications,
                Statuses = statuses,
                Users = users
            };
            return View(viewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Form(BugReportFormViewModel model)
    {
        if (!ModelState.IsValid)
        {

            model.Priorities = await _bugReportService.GetBugPrioritiesAsync();
            model.Applications = await _bugReportService.GetApplicationsAsync();
            model.Statuses = await _bugReportService.GetBugStatusesAsync();
            model.Users = await GetAssignableUsersAsync(); // Filter assignable users
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        if (model.Id == null)
        {
            // Create operation
            var bugReport = new BugReport
            {
                Title = model.Title,
                Description = model.Description,
                PriorityId = model.PriorityId,
                ApplicationId = model.ApplicationId,
                StatusId = 1, // New
                ReporterId = user.Id,
                AssignedToUserId = model.AssignedToUserId
            };

            await _bugReportService.CreateAsync(bugReport, user);

            // Notify QA team
            await _emailSender.SendEmailAsync("qa-team@example.com", "New Bug Report Created", $"A new bug report '{bugReport.Title}' has been created.");

            TempData["SuccessMessage"] = "Bug report created successfully!";
        }
        else
        {
            // Edit operation
            var bugReport = await _bugReportService.GetByIdAsync(model.Id.Value);
            if (bugReport == null) return NotFound();

            // Update properties
            bugReport.Title = model.Title;
            bugReport.Description = model.Description;
            bugReport.PriorityId = model.PriorityId;
            bugReport.ApplicationId = model.ApplicationId;

            // Handle status change
            if (model.StatusId != bugReport.StatusId)
            {
                bugReport.ChangeStatus(model.StatusId, userRole);

                // Notify based on status
                if (model.StatusId == 2) // In Test
                {
                    await _emailSender.SendEmailAsync("qa-team@example.com", "Bug Report Assigned to QA", $"The bug report '{bugReport.Title}' is now in testing.");
                }
                else if (model.StatusId == 3) // In Development
                {
                    var developer = await _userManager.FindByIdAsync(bugReport.AssignedToUserId);
                    await _emailSender.SendEmailAsync(developer.Email, "Bug Report Assigned to Developer", $"The bug report '{bugReport.Title}' is now in development.");
                }
                else if (model.StatusId == 4) // Ready for Regression
                {
                    await _emailSender.SendEmailAsync("qa-team@example.com", "Bug Report Ready for Regression", $"The bug report '{bugReport.Title}' is ready for regression testing.");
                }
            }

            await _bugReportService.UpdateAsync(bugReport, user);

            TempData["SuccessMessage"] = "Bug report updated successfully!";
        }

        // Redirect to the List action
        return RedirectToAction("List");
    }

    private async Task<List<AppUser>> GetAssignableUsersAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        if (userRole == "Admin")
        {
            // Admin can assign to QA and Developer
            var qaUsers = await _userManager.GetUsersInRoleAsync("QA");
            var devUsers = await _userManager.GetUsersInRoleAsync("Developer");
            return qaUsers.Concat(devUsers).ToList();
        }
        else if (userRole == "QA")
        {
            // QA can assign to Developer
            var devUsers = await _userManager.GetUsersInRoleAsync("Developer");
            return devUsers.ToList();
        }
        else if (userRole == "User")
        {
            // User can assign to QA
            var qaUsers = await _userManager.GetUsersInRoleAsync("QA");
            return qaUsers.ToList();
        }

        return new List<AppUser>();
    }
}
