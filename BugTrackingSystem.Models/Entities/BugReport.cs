using BugTrackingSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.Models.Entities
{
    public class BugReport
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Enums for status and priority
        public BugStatus Status { get; set; } = BugStatus.Open;

        public BugPriority Priority { get; set; } = BugPriority.Medium;

        // Foreign Key to Reporter (User)
        [Required]
        public string ReporterId { get; set; }

        public AppUser Reporter { get; set; }

        // Foreign Key to Project
        public int ProjectId { get; set; }

        public ApplicationName ApplicationName { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
