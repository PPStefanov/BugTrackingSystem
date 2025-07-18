using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTrackingSystem.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public ICollection<BugReport> ReportedBugs { get; set; }
        
        // Navigation properties for different assignment types
        public ICollection<BugReport> AssignedBugs { get; set; } = new List<BugReport>();
        public ICollection<BugReport> DeveloperBugs { get; set; } = new List<BugReport>();
        
        // Admin subscriptions
        public ICollection<AdminSubscription> AdminSubscriptions { get; set; } = new List<AdminSubscription>();
    }
}
