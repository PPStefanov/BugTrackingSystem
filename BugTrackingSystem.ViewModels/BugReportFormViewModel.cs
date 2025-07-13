namespace BugTrackingSystem.ViewModels
{
    using BugTrackingSystem.Models.Entities;
    using System.ComponentModel.DataAnnotations;

    public class BugReportFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required]
        public int PriorityId { get; set; } // Foreign key for BugPriority
        public List<BugPriorityEntity> Priorities { get; set; } // For dropdown

        [Required]
        public int ApplicationId { get; set; } // Foreign key for ApplicationName
        public List<ApplicationName> Applications { get; set; } // For dropdown

        public string AssignedToUserId { get; set; } // Optional field for assignment
        public List<AppUser> Users { get; set; } // For dropdown (if needed)

        [Required]
        public int StatusId { get; set; } // Foreign key for BugStatusEntity
        public List<BugStatusEntity> Statuses { get; set; } // For dropdown
    }
}

