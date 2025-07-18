using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Services.Core
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, int? bugReportId = null);
        Task<List<Notification>> GetUserNotificationsAsync(string userId, int count = 10);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteNotificationAsync(int notificationId);
        
        // New methods for enhanced notification logic
        Task NotifyBugReportStakeholdersAsync(int bugReportId, string title, string message, NotificationType type, string? excludeUserId = null);
        Task<bool> IsAdminSubscribedToTicketAsync(string adminId, int bugReportId);
        Task SubscribeAdminToTicketAsync(string adminId, int bugReportId);
        Task UnsubscribeAdminFromTicketAsync(string adminId, int bugReportId);
        Task<List<int>> GetAdminSubscribedTicketsAsync(string adminId);
    }
}