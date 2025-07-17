using BugTrackingSystem.GCommon;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models.Entities
{
    public class BugReport
    {
        public int Id { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.BugReportValidation.TitleMaxLength,
            MinimumLength = ValidationConstants.BugReportValidation.TitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.BugReportValidation.DescriptionMaxLength,
            MinimumLength = ValidationConstants.BugReportValidation.DescriptionMinLength)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int StatusId { get; set; } // Foreign key for BugStatusEntity
        public BugStatusEntity Status { get; set; } // Navigation property

        public int PriorityId { get; set; }
        public BugPriorityEntity Priority { get; set; }

        [Required]
        public string ReporterId { get; set; }
        public AppUser Reporter { get; set; }

        public int ApplicationId { get; set; }
        public ApplicationName ApplicationName { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public string AssignedToUserId { get; set; }
        public AppUser AssignedToUser { get; set; }

        public void ChangeStatus(int newStatusId, string role)
        {
            // Define valid transitions
            var validTransitions = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 2 } }, // New → In Test
                { 2, new List<int> { 3 } }, // In Test → In Development
                { 3, new List<int> { 4 } }, // In Development → Ready for Regression
                { 4, new List<int> { 5 } }  // Ready for Regression → Closed
            };

            if (!validTransitions.ContainsKey(StatusId) || !validTransitions[StatusId].Contains(newStatusId))
            {
                throw new InvalidOperationException("Invalid status transition.");
            }

            // Additional role-based rules
            if (newStatusId == 5 && role != "QA") // Only QA can close the ticket
            {
                throw new InvalidOperationException("Only QA can close the ticket.");
            }

            StatusId = newStatusId;
        }

        public bool IsOpenedByQA { get; set; } = false;
    }
}