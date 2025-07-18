using BugTrackingSystem.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSubscriptionController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<AppUser> _userManager;

        public AdminSubscriptionController(INotificationService notificationService, UserManager<AppUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int bugReportId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            try
            {
                await _notificationService.SubscribeAdminToTicketAsync(userId, bugReportId);
                return Json(new { success = true, message = "Successfully subscribed to ticket notifications" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to subscribe: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int bugReportId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            try
            {
                await _notificationService.UnsubscribeAdminFromTicketAsync(userId, bugReportId);
                return Json(new { success = true, message = "Successfully unsubscribed from ticket notifications" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to unsubscribe: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsSubscribed(int bugReportId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { isSubscribed = false });
            }

            var isSubscribed = await _notificationService.IsAdminSubscribedToTicketAsync(userId, bugReportId);
            return Json(new { isSubscribed });
        }
    }
}