using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Models.Enums;
using BugTrackingSystem.ViewModels.BugReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> List(BugReportFilterViewModel filters)
        {
            var user = await _userManager.GetUserAsync(User);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var userId = user.Id;
            var priorities = await _bugReportService.GetBugPrioritiesAsync();

            // Get all tickets based on user role
            List<BugReportEntity> allTickets;
            if (userRole == "Admin" || userRole == "QA")
            {
                allTickets = (await _bugReportService.GetAllBugsAsync())
                    .Where(t => t.Status?.Name != "Closed")
                    .ToList();
            }
            else if (userRole == "Developer")
            {
                allTickets = (await _bugReportService.GetBugsByStatusAsync("In Development"))
                    .Where(t => t.Status?.Name != "Closed")
                    .ToList();
            }
            else
            {
                allTickets = (await _bugReportService.GetBugsForUserAsync(user))
                    .Where(t => t.Status?.Name != "Closed")
                    .ToList();
            }

            // Apply filters
            var filteredTickets = ApplyFilters(allTickets, filters);

            // Apply sorting
            filteredTickets = ApplySorting(filteredTickets, filters.SortBy, filters.SortOrder);

            // Build ViewModels
            var bugReportViewModels = BuildBugReportListViewModel(filteredTickets, priorities, userRole, userId);
            
            // Prepare filter options
            await PopulateFilterOptions(filters, allTickets);

            // Create page ViewModel
            var pageViewModel = new BugReportListPageViewModel
            {
                BugReports = bugReportViewModels,
                Filters = filters,
                TotalCount = allTickets.Count,
                FilteredCount = filteredTickets.Count,
                HasFiltersApplied = HasFiltersApplied(filters)
            };

            return View(pageViewModel);
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
                Status = ticket.Status?.Name ?? "Unknown",
                Priority = ticket.Priority?.Name ?? priorities.FirstOrDefault(p => p.Id == ticket.PriorityId)?.Name ?? "Unknown",
                Application = ticket.ApplicationName?.Name ?? "Unknown",
                AssignedTo = ticket.AssignedToUser?.UserName ?? "Unassigned",
                Reporter = ticket.Reporter?.UserName ?? "Unknown",
                CreatedDate = ticket.CreatedAt,
                LastEditDate = ticket.UpdatedAt,
                LastEditUser = ticket.UpdatedAt.HasValue ? "System" : string.Empty,
                CanEdit =
                    userRole == "Admin" ||
                    (userRole == "User" && ticket.ReporterId == userId && ticket.Status?.Name == "New") ||
                    (userRole == "QA" && ticket.Status?.Name != "In Development") ||
                    (userRole == "Developer" && ticket.Status?.Name == "In Development")
            }).ToList();
        }

        private List<BugReportEntity> ApplyFilters(List<BugReportEntity> tickets, BugReportFilterViewModel filters)
        {
            var filteredTickets = tickets.AsEnumerable();

            // Search term filter
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                filteredTickets = filteredTickets.Where(t => 
                    t.Title.ToLower().Contains(searchTerm) ||
                    t.Description.ToLower().Contains(searchTerm) ||
                    (t.Reporter?.Email?.ToLower().Contains(searchTerm) ?? false) ||
                    (t.AssignedToUser?.Email?.ToLower().Contains(searchTerm) ?? false));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(filters.StatusFilter))
            {
                filteredTickets = filteredTickets.Where(t => t.Status?.Name == filters.StatusFilter);
            }

            // Priority filter
            if (!string.IsNullOrWhiteSpace(filters.PriorityFilter))
            {
                filteredTickets = filteredTickets.Where(t => t.Priority?.Name == filters.PriorityFilter);
            }

            // Application filter
            if (!string.IsNullOrWhiteSpace(filters.ApplicationFilter))
            {
                filteredTickets = filteredTickets.Where(t => t.ApplicationName?.Name == filters.ApplicationFilter);
            }

            // Assigned To filter
            if (!string.IsNullOrWhiteSpace(filters.AssignedToFilter))
            {
                filteredTickets = filteredTickets.Where(t => t.AssignedToUser?.Email == filters.AssignedToFilter);
            }

            // Reporter filter
            if (!string.IsNullOrWhiteSpace(filters.ReporterFilter))
            {
                filteredTickets = filteredTickets.Where(t => t.Reporter?.Email == filters.ReporterFilter);
            }

            // Date range filter
            if (filters.DateFrom.HasValue)
            {
                filteredTickets = filteredTickets.Where(t => t.CreatedAt >= filters.DateFrom.Value);
            }

            if (filters.DateTo.HasValue)
            {
                var dateTo = filters.DateTo.Value.AddDays(1); // Include the entire day
                filteredTickets = filteredTickets.Where(t => t.CreatedAt < dateTo);
            }

            return filteredTickets.ToList();
        }

        private List<BugReportEntity> ApplySorting(List<BugReportEntity> tickets, string sortBy, string sortOrder)
        {
            var sortedTickets = tickets.AsEnumerable();
            var isDescending = sortOrder?.ToLower() == "desc";

            sortedTickets = sortBy?.ToLower() switch
            {
                "title" => isDescending ? sortedTickets.OrderByDescending(t => t.Title) : sortedTickets.OrderBy(t => t.Title),
                "status" => isDescending ? sortedTickets.OrderByDescending(t => t.Status?.Name ?? "") : sortedTickets.OrderBy(t => t.Status?.Name ?? ""),
                "priority" => isDescending ? sortedTickets.OrderByDescending(t => t.Priority?.Name ?? "") : sortedTickets.OrderBy(t => t.Priority?.Name ?? ""),
                "application" => isDescending ? sortedTickets.OrderByDescending(t => t.ApplicationName?.Name ?? "") : sortedTickets.OrderBy(t => t.ApplicationName?.Name ?? ""),
                "assignedto" => isDescending ? sortedTickets.OrderByDescending(t => t.AssignedToUser?.Email ?? "") : sortedTickets.OrderBy(t => t.AssignedToUser?.Email ?? ""),
                "reporter" => isDescending ? sortedTickets.OrderByDescending(t => t.Reporter?.Email ?? "") : sortedTickets.OrderBy(t => t.Reporter?.Email ?? ""),
                "lastedit" => isDescending ? sortedTickets.OrderByDescending(t => t.UpdatedAt ?? DateTime.MinValue) : sortedTickets.OrderBy(t => t.UpdatedAt ?? DateTime.MinValue),
                "createddate" or _ => isDescending ? sortedTickets.OrderByDescending(t => t.CreatedAt) : sortedTickets.OrderBy(t => t.CreatedAt)
            };

            return sortedTickets.ToList();
        }

        private async Task PopulateFilterOptions(BugReportFilterViewModel filters, List<BugReportEntity> allTickets)
        {
            // Status options
            var statuses = await _bugReportService.GetBugStatusesAsync();
            filters.StatusOptions = statuses.Select(s => new SelectListItem
            {
                Value = s.Name,
                Text = s.Name,
                Selected = s.Name == filters.StatusFilter
            }).Prepend(new SelectListItem { Value = "", Text = "All Statuses" });

            // Priority options
            var priorities = await _bugReportService.GetBugPrioritiesAsync();
            filters.PriorityOptions = priorities.Select(p => new SelectListItem
            {
                Value = p.Name,
                Text = p.Name,
                Selected = p.Name == filters.PriorityFilter
            }).Prepend(new SelectListItem { Value = "", Text = "All Priorities" });

            // Application options
            var applications = await _bugReportService.GetApplicationsAsync();
            filters.ApplicationOptions = applications.Select(a => new SelectListItem
            {
                Value = a.Name,
                Text = a.Name,
                Selected = a.Name == filters.ApplicationFilter
            }).Prepend(new SelectListItem { Value = "", Text = "All Applications" });

            // Assigned To options
            var assignedToUsers = allTickets
                .Where(t => t.AssignedToUser != null)
                .Select(t => t.AssignedToUser.Email)
                .Distinct()
                .OrderBy(email => email);
            filters.AssignedToOptions = assignedToUsers.Select(email => new SelectListItem
            {
                Value = email,
                Text = email,
                Selected = email == filters.AssignedToFilter
            }).Prepend(new SelectListItem { Value = "", Text = "All Assigned Users" });

            // Reporter options
            var reporters = allTickets
                .Where(t => t.Reporter != null)
                .Select(t => t.Reporter.Email)
                .Distinct()
                .OrderBy(email => email);
            filters.ReporterOptions = reporters.Select(email => new SelectListItem
            {
                Value = email,
                Text = email,
                Selected = email == filters.ReporterFilter
            }).Prepend(new SelectListItem { Value = "", Text = "All Reporters" });

            // Sort By options
            filters.SortByOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "createddate", Text = "Created Date", Selected = filters.SortBy == "createddate" },
                new SelectListItem { Value = "lastedit", Text = "Last Edit Date", Selected = filters.SortBy == "lastedit" },
                new SelectListItem { Value = "title", Text = "Title", Selected = filters.SortBy == "title" },
                new SelectListItem { Value = "status", Text = "Status", Selected = filters.SortBy == "status" },
                new SelectListItem { Value = "priority", Text = "Priority", Selected = filters.SortBy == "priority" },
                new SelectListItem { Value = "application", Text = "Application", Selected = filters.SortBy == "application" },
                new SelectListItem { Value = "assignedto", Text = "Assigned To", Selected = filters.SortBy == "assignedto" },
                new SelectListItem { Value = "reporter", Text = "Reporter", Selected = filters.SortBy == "reporter" }
            };
        }

        private bool HasFiltersApplied(BugReportFilterViewModel filters)
        {
            return !string.IsNullOrWhiteSpace(filters.SearchTerm) ||
                   !string.IsNullOrWhiteSpace(filters.StatusFilter) ||
                   !string.IsNullOrWhiteSpace(filters.PriorityFilter) ||
                   !string.IsNullOrWhiteSpace(filters.ApplicationFilter) ||
                   !string.IsNullOrWhiteSpace(filters.AssignedToFilter) ||
                   !string.IsNullOrWhiteSpace(filters.ReporterFilter) ||
                   filters.DateFrom.HasValue ||
                   filters.DateTo.HasValue;
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