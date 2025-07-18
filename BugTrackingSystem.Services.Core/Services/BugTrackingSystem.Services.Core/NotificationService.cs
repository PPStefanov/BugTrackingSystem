using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Services.Core
{
    public class NotificationService : INotificationService
    {
        private readonly BugTrackingSystemDbContext _context;

        public NotificationService(BugTrackingSystemDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, int? bugReportId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                BugReportId = bugReportId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId, int count = 10)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .Include(n => n.BugReport)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task NotifyBugReportStakeholdersAsync(int bugReportId, string title, string message, NotificationType type, string? excludeUserId = null)
        {
            var bugReport = await _context.BugReports
                .Include(b => b.Reporter)
                .Include(b => b.AssignedToUser)
                .Include(b => b.Developer)
                .FirstOrDefaultAsync(b => b.Id == bugReportId);

            if (bugReport == null) return;

            var stakeholders = new List<string>();

            // Add ticket creator
            if (!string.IsNullOrEmpty(bugReport.ReporterId) && bugReport.ReporterId != excludeUserId)
            {
                stakeholders.Add(bugReport.ReporterId);
            }

            // Add assigned QA
            if (!string.IsNullOrEmpty(bugReport.AssignedToUserId) && bugReport.AssignedToUserId != excludeUserId)
            {
                stakeholders.Add(bugReport.AssignedToUserId);
            }

            // Add assigned Developer
            if (!string.IsNullOrEmpty(bugReport.DeveloperId) && bugReport.DeveloperId != excludeUserId)
            {
                stakeholders.Add(bugReport.DeveloperId);
            }

            // Add subscribed admins
            var subscribedAdmins = await _context.AdminSubscriptions
                .Where(s => s.BugReportId == bugReportId && s.IsActive)
                .Select(s => s.AdminId)
                .ToListAsync();

            foreach (var adminId in subscribedAdmins)
            {
                if (adminId != excludeUserId && !stakeholders.Contains(adminId))
                {
                    stakeholders.Add(adminId);
                }
            }

            // Create notifications for all stakeholders
            foreach (var userId in stakeholders.Distinct())
            {
                await CreateNotificationAsync(userId, title, message, type, bugReportId);
            }
        }

        public async Task<bool> IsAdminSubscribedToTicketAsync(string adminId, int bugReportId)
        {
            return await _context.AdminSubscriptions
                .AnyAsync(s => s.AdminId == adminId && s.BugReportId == bugReportId && s.IsActive);
        }

        public async Task SubscribeAdminToTicketAsync(string adminId, int bugReportId)
        {
            var existingSubscription = await _context.AdminSubscriptions
                .FirstOrDefaultAsync(s => s.AdminId == adminId && s.BugReportId == bugReportId);

            if (existingSubscription != null)
            {
                existingSubscription.IsActive = true;
            }
            else
            {
                var subscription = new AdminSubscription
                {
                    AdminId = adminId,
                    BugReportId = bugReportId,
                    SubscribedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.AdminSubscriptions.Add(subscription);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeAdminFromTicketAsync(string adminId, int bugReportId)
        {
            var subscription = await _context.AdminSubscriptions
                .FirstOrDefaultAsync(s => s.AdminId == adminId && s.BugReportId == bugReportId);

            if (subscription != null)
            {
                subscription.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<int>> GetAdminSubscribedTicketsAsync(string adminId)
        {
            return await _context.AdminSubscriptions
                .Where(s => s.AdminId == adminId && s.IsActive)
                .Select(s => s.BugReportId)
                .ToListAsync();
        }
    }
}