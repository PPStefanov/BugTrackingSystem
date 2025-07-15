namespace BugTrackingSystem.ViewModels
{
    using BugTrackingSystem.Models.Entities;
    using System.ComponentModel.DataAnnotations;

    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
}

