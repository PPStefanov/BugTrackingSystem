using System.ComponentModel.DataAnnotations;
using BugTrackingSystem.GCommon;

namespace BugTrackingSystem.ViewModels.Comment
{
    public class EditCommentViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(
            ValidationConstants.CommentValidation.ContentMaxLength,
            MinimumLength = ValidationConstants.CommentValidation.ContentMinLength)]
        public string Content { get; set; }

        public int BugReportId { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}