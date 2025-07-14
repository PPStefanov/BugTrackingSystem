using BugTrackingSystem.GCommon;
using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.CommentValidation.ContentMaxLength,
            MinimumLength = ValidationConstants.CommentValidation.ContentMinLength)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string AuthorId { get; set; }

        public AppUser Author { get; set; }

        public int BugReportId { get; set; }

        public BugReport BugReport { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
