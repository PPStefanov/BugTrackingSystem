using BugTrackingSystem.GCommon;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models.Entities
{
    public class ApplicationName
    {
        public int Id { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.ApplicationNameValidation.NameMaxLength,
            MinimumLength = ValidationConstants.ApplicationNameValidation.NameMinLength)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<BugReport> BugReports { get; set; } = new List<BugReport>();
    }
}
