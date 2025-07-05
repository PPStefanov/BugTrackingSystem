using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTrackingSystem.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public ICollection<BugReport> ReportedBugs { get; set; }
    }
}
