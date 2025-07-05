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
            ValidationConstants.ApplicationName.NameMaxLength,
            MinimumLength = ValidationConstants.ApplicationName.NameMinLength)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<BugReport> BugReports { get; set; }
    }
}
