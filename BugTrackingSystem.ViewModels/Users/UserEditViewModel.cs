using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTrackingSystem.GCommon;


namespace BugTrackingSystem.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(
        ValidationConstants.AppUserValidation.PasswordMaxLength,
        MinimumLength = ValidationConstants.AppUserValidation.PasswordMinLength,
        ErrorMessage = "Password must be between {2} and {1} characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}