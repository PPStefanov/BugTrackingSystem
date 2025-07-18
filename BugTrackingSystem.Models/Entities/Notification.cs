using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public AppUser User { get; set; } = null!;
        
        public int? BugReportId { get; set; }
        
        public BugReport? BugReport { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRead { get; set; } = false;
        
        public NotificationType Type { get; set; }
    }
    
    public enum NotificationType
    {
        BugReportCreated,
        BugReportStatusChanged,
        BugReportAssigned,
        CommentAdded,
        BugReportUpdated
    }
}