using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace BugTrackingSystem.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentService commentService, UserManager<AppUser> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
            {
                TempData["ErrorMessage"] = "Comment not found.";
                return RedirectToAction("List", "BugReport");
            }

            var success = await _commentService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Comment deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete comment.";
            }

            // Redirect back to the bug report details page
            return RedirectToAction("Details", "BugReport", new { id = comment.BugReportId });
        }

        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
            {
                TempData["ErrorMessage"] = "Comment not found.";
                return RedirectToAction("List", "BugReport");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            // Only allow editing if user is admin or the comment author
            if (!isAdmin && comment.AuthorId != currentUser?.Id)
            {
                TempData["ErrorMessage"] = "You don't have permission to edit this comment.";
                return RedirectToAction("Details", "BugReport", new { id = comment.BugReportId });
            }

            var viewModel = new EditCommentViewModel
            {
                Id = comment.Id,
                Content = comment.Content,
                BugReportId = comment.BugReportId,
                AuthorId = comment.AuthorId,
                CreatedAt = comment.CreatedAt,
                IsActive = comment.IsActive
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(EditCommentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Add debugging info
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        TempData["ErrorMessage"] = $"Validation error: {error.ErrorMessage}";
                    }
                    return View(model);
                }

                var existingComment = await _commentService.GetByIdAsync(model.Id);
                if (existingComment == null)
                {
                    TempData["ErrorMessage"] = "Comment not found.";
                    return RedirectToAction("List", "BugReport");
                }

                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = User.IsInRole("Admin");

                // Only allow editing if user is admin or the comment author
                if (!isAdmin && existingComment.AuthorId != currentUser?.Id)
                {
                    TempData["ErrorMessage"] = "You don't have permission to edit this comment.";
                    return RedirectToAction("Details", "BugReport", new { id = existingComment.BugReportId });
                }

                existingComment.Content = model.Content;
                var success = await _commentService.UpdateAsync(existingComment);

                if (success)
                {
                    TempData["SuccessMessage"] = "Comment updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update comment.";
                }

                return RedirectToAction("Details", "BugReport", new { id = model.BugReportId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("List", "BugReport");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int bugReportId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Comment content cannot be empty.";
                return RedirectToAction("Details", "BugReport", new { id = bugReportId });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to add comments.";
                return RedirectToAction("Details", "BugReport", new { id = bugReportId });
            }

            var comment = new Comment
            {
                Content = content,
                BugReportId = bugReportId,
                AuthorId = currentUser.Id,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var createdComment = await _commentService.CreateAsync(comment);
            if (createdComment != null)
            {
                TempData["SuccessMessage"] = "Comment added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add comment.";
            }

            return RedirectToAction("Details", "BugReport", new { id = bugReportId });
        }
    }
}