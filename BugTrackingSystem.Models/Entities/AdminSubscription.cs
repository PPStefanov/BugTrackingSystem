using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models.Entities
{
    public class AdminSubscription
    {
        public int Id { get; set; }

        [Required]
        public string AdminId { get; set; }
        public AppUser Admin { get; set; }

        public int BugReportId { get; set; }
        public BugReport BugReport { get; set; }

        public DateTime SubscribedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}