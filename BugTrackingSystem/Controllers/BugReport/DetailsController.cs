using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DetailsController : Controller
{
    private readonly IBugReportService _bugReportService;
    private readonly ICommentService _commentService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public DetailsController(
        IBugReportService bugReportService,
        ICommentService commentService,
        UserManager<AppUser> userManager,
        IEmailSender emailSender)
    {
        _bugReportService = bugReportService;
        _commentService = commentService;
        _userManager = userManager;
        _emailSender = emailSender;
    }


    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var bugReport = await _bugReportService.GetByIdAsync(id);
        if (bugReport == null) return NotFound();

        // Load comments/messages (assuming Comments navigation property is loaded)
        //return View("Details", bugReport);
        return View("~/Views/BugReport/Details.cshtml", bugReport);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int bugReportId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Message cannot be empty.";
            return RedirectToAction("Index", new { id = bugReportId });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var comment = new Comment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            AuthorId = user.Id,
            IsActive = true,
            BugReportId = bugReportId
        };

        await _commentService.CreateAsync(comment);

        TempData["Success"] = "Message added.";
        return RedirectToAction("Index", new { id = bugReportId });
    }

    [HttpGet]
    public async Task<IActionResult> EditComment(int id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = user != null && (await _userManager.GetRolesAsync(user)).Contains("Admin");
        if (user == null || (comment.AuthorId != user.Id && !isAdmin))
            return Forbid();

        return View(comment);
    }

    [HttpPost]
    public async Task<IActionResult> EditComment(int id, string content)
    {
        var comment = await _commentService.GetByIdAsync(id);
        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = user != null && (await _userManager.GetRolesAsync(user)).Contains("Admin");
        if (user == null || (comment.AuthorId != user.Id && !isAdmin))
            return Forbid();

        comment.Content = content;
        await _commentService.UpdateAsync(comment);

        TempData["Success"] = "Message updated.";
        return RedirectToAction("Index", new { id = comment.BugReportId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = user != null && (await _userManager.GetRolesAsync(user)).Contains("Admin");
        if (user == null || (comment.AuthorId != user.Id && !isAdmin))
            return Forbid();

        await _commentService.DeleteAsync(id);

        TempData["Success"] = "Message deleted.";
        return RedirectToAction("Index", new { id = comment.BugReportId });
    }

    // For file uploads, you would add another action here (not shown for brevity)
}