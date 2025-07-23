using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Data;
using Microsoft.AspNetCore.Identity;
using BugTrackingSystem.ViewModels.BugReport;

public class BugReportService : IBugReportService
{
    private readonly BugTrackingSystemDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public BugReportService(BugTrackingSystemDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    //public async Task<List<BugReport>> GetBugsForUserAsync(AppUser user)
    //{
    //    if (await _userManager.IsInRoleAsync(user, "Admin"))
    //    {
    //        return await _dbContext.BugReports
    //            .Where(b => b.IsActive)
    //            .Include(b => b.AssignedToUser)
    //            .ToListAsync();
    //    }
    //    else
    //    {
    //        return await _dbContext.BugReports
    //            .Where(b => b.IsActive && b.AssignedToUserId == user.Id)
    //            .Include(b => b.AssignedToUser)
    //            .ToListAsync();
    //    }
    //}

    //public async Task<List<BugReport>> GetAllBugsAsync()
    //{
    //    return await _dbContext.BugReports.Include(b => b.AssignedToUser).ToListAsync();
    //}

    public async Task<List<BugReport>> GetAllBugsAsync()
    {
        //return await _dbContext.BugReports.ToListAsync();
        return await _dbContext.BugReports
                    .Include(b => b.Status)
                    .Include(b => b.Priority)
                    .Include(b => b.ApplicationName)
                    .Include(b => b.AssignedToUser)
                    .Include(b => b.Reporter)
                    .ToListAsync();
    }

    public async Task<List<BugReport>> GetBugsForUserAsync(AppUser user)
    {
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return await _dbContext.BugReports
                .Where(b => b.IsActive && b.ReporterId == user.Id)
                .Include(b => b.Status)
                .Include(b => b.Priority)
                .Include(b => b.ApplicationName)
                .Include(b => b.AssignedToUser)
                .Include(b => b.Reporter)
                .ToListAsync();
        }
        else
        {
            return await _dbContext.BugReports
                .Where(b => b.IsActive && b.AssignedToUserId == user.Id || b.ReporterId == user.Id)
                .Include(b => b.Status)
                .Include(b => b.Priority)
                .Include(b => b.ApplicationName)
                .Include(b => b.AssignedToUser)
                .Include(b => b.Reporter)
                .ToListAsync();
        }
    }

    //public async Task<BugReport> GetByIdAsync(int id)
    //{
    //    return await _dbContext.BugReports
    //        .Include(b => b.AssignedToUser)
    //        .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
    //}

    public async Task<BugReport> GetByIdAsync(int id)
    {
        return await _dbContext.BugReports
            .Include(b => b.Status)
            .Include(b => b.Priority)
            .Include(b => b.ApplicationName)
            .Include(b => b.AssignedToUser)
            .Include(b => b.Reporter)
            .Include(b => b.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
    }

    public async Task<BugReport> CreateAsync(BugReport bugReport, AppUser creator)
    {
        // Always set the creator as the reporter (if you have such a property)
        bugReport.Reporter = creator;
        bugReport.ReporterId = creator.Id;

        // Only allow assignment to self or (if QA/Admin) to others
        if (!string.IsNullOrEmpty(bugReport.AssignedToUserId) && bugReport.AssignedToUserId != creator.Id)
        {
            var isQAOrAdmin = await _userManager.IsInRoleAsync(creator, "QA") ||
                              await _userManager.IsInRoleAsync(creator, "Admin");
            if (!isQAOrAdmin)
                bugReport.AssignedToUserId = creator.Id; // Fallback to self
        }
        _dbContext.BugReports.Add(bugReport);
        await _dbContext.SaveChangesAsync();
        return bugReport;
    }

    public async Task<bool> UpdateAsync(BugReport bugReport, AppUser updater)
    {
        var original = await _dbContext.BugReports.FindAsync(bugReport.Id);
        if (original == null) return false;

        // Only allow updates if the updater is:
        // - Admin, or
        // - The current assignee, or
        // - The original reporter (depending on your rules)
        var isAdmin = await _userManager.IsInRoleAsync(updater, "Admin");
        if (!isAdmin && original.AssignedToUserId != updater.Id && original.ReporterId != updater.Id)
            return false;

        // Update mutable properties
        original.Title = bugReport.Title;
        original.Description = bugReport.Description;
        original.Priority = bugReport.Priority;
        original.Status = bugReport.Status;

        // Assignment: only Admin or QA can reassign
        if ((await _userManager.IsInRoleAsync(updater, "Admin") || await _userManager.IsInRoleAsync(updater, "QA"))
            && !string.IsNullOrEmpty(bugReport.AssignedToUserId))
        {
            original.AssignedToUserId = bugReport.AssignedToUserId;
        }

        _dbContext.BugReports.Update(original);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, AppUser deleter)
    {
        var bug = await _dbContext.BugReports.FindAsync(id);
        if (bug == null) return false;

        // Only Admin can delete (or you can relax this)
        if (!await _userManager.IsInRoleAsync(deleter, "Admin"))
            return false;

        bug.IsActive = false;
        _dbContext.BugReports.Update(bug);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignToUserAsync(int bugId, string assignedToUserId, AppUser assigner)
    {
        var bug = await _dbContext.BugReports.FindAsync(bugId);
        if (bug == null) return false;

        // Only QA or Admin can assign
        var isQAOrAdmin = await _userManager.IsInRoleAsync(assigner, "QA") ||
                          await _userManager.IsInRoleAsync(assigner, "Admin");
        if (!isQAOrAdmin) return false;

        bug.AssignedToUserId = assignedToUserId;
        _dbContext.BugReports.Update(bug);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<BugPriorityEntity>> GetBugPrioritiesAsync()
    {
        return await _dbContext.BugPriorities.ToListAsync();
    }

    public async Task<List<ApplicationName>> GetApplicationsAsync()
    {
        return await _dbContext.ApplicationName.Where(a => a.IsActive).ToListAsync();
    }

    public async Task<List<BugStatusEntity>> GetBugStatusesAsync()
    {
        return await _dbContext.BugStatuses.ToListAsync();
    }

    public async Task<BugStatusEntity?> GetBugStatusByIdAsync(int statusId)
    {
        return await _dbContext.BugStatuses.FindAsync(statusId);
    }


    //public async Task<List<BugReport>> GetBugsByStatusAsync(string statusName)
    //{
    //    return await _dbContext.BugReports
    //        .Include(b => b.Status) // Include the related BugStatusEntity
    //        //.Include(b => b.Priority) // Include Priority for additional details
    //        .Include(b => b.ApplicationName) // Include ApplicationName for additional details
    //        .Include(b => b.AssignedToUser) // Include AssignedToUser for additional details
    //        .Where(b => b.Status.Name == statusName && b.IsActive) // Filter by status name and active bugs
    //        .ToListAsync();
    //}

    public async Task<List<BugReport>> GetBugsByStatusAsync(string statusName)
    {
        return await _dbContext.BugReports
            .Include(b => b.Status)
            .Include(b => b.Priority)
            .Include(b => b.ApplicationName)
            .Include(b => b.AssignedToUser)
            .Include(b => b.Reporter)
            .Where(b => b.Status.Name == statusName && b.IsActive)
            .ToListAsync();
    }

    public async Task<List<BugReportListViewModel>> GetAllBugReportViewModelsAsync()
    {
        var statuses = await _dbContext.BugStatuses.ToDictionaryAsync(s => s.Id, s => s.Name);
        var priorities = await _dbContext.BugPriorities.ToDictionaryAsync(p => p.Id, p => p.Name);
        var applications = await _dbContext.ApplicationName.ToDictionaryAsync(a => a.Id, a => a.Name);

        var tickets = await _dbContext.BugReports
            .Include(b => b.AssignedToUser)
            .Where(b => b.IsActive)
            .ToListAsync();

        return tickets.Select(ticket => new BugReportListViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Status = statuses.TryGetValue(ticket.StatusId, out var statusName) ? statusName : "",
            Priority = priorities.TryGetValue(ticket.PriorityId, out var priorityName) ? priorityName : ticket.Priority.ToString(),
            Application = applications.TryGetValue(ticket.ApplicationId, out var appName) ? appName : "",
            AssignedTo = ticket.AssignedToUser?.UserName ?? "Unassigned"
        }).ToList();
    }


    public async Task<bool> AutoAdvanceStatusToInTestIfQAAsync(BugReport bugReport, AppUser user, List<BugStatusEntity> statuses)
    {
        // Only advance if QA, status is "New", and not already opened by QA
        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains("QA") && bugReport.Status?.Name == "New" && !bugReport.IsOpenedByQA)
        {
            var inTestStatus = statuses.FirstOrDefault(s => s.Name == "In Test");
            if (inTestStatus != null)
            {
                bugReport.StatusId = inTestStatus.Id;
                bugReport.IsOpenedByQA = true;
                await UpdateAsync(bugReport, user);
                return true;
            }
        }
        return false;
    }
}