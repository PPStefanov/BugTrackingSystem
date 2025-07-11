using BugTrackingSystem.GCommon;
using BugTrackingSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models.Entities
{
    public class BugReport
    {
        public int Id { get; set; }

        // Foreign key to Bug entity
        public int BugId { get; set; }
        public Bug Bug { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.BugReport.TitleMaxLength,
            MinimumLength = ValidationConstants.BugReport.TitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.BugReport.DescriptionMaxLength,
            MinimumLength = ValidationConstants.BugReport.DescriptionMinLength)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public BugStatus Status { get; set; } = BugStatus.Open;

        public BugPriority Priority { get; set; } = BugPriority.Medium;

        [Required]
        public string ReporterId { get; set; }

        public AppUser Reporter { get; set; }

        public int ProjectId { get; set; }

        public ApplicationName ApplicationName { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public string AssignedToUserId { get; set; }
        public AppUser AssignedToUser { get; set; }
    }
}