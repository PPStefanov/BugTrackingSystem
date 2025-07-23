using System.ComponentModel.DataAnnotations;
using BugTrackingSystem.GCommon;

namespace BugTrackingSystem.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New password is required")]
        [StringLength(
            ValidationConstants.AppUserValidation.PasswordMaxLength,
            MinimumLength = ValidationConstants.AppUserValidation.PasswordMinLength,
            ErrorMessage = "Password must be between {2} and {1} characters.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}