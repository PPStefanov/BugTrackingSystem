using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BugTrackingSystem.Models.Entities;    

public class BugReportController : Controller
{
    private readonly IBugReportService _bugReportService;
    private readonly UserManager<AppUser> _userManager;

    public BugReportController(IBugReportService bugReportService, UserManager<AppUser> userManager)
    {
        _bugReportService = bugReportService;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var bugs = await _bugReportService.GetBugsForUserAsync(user);
        return View(bugs);
    }
}