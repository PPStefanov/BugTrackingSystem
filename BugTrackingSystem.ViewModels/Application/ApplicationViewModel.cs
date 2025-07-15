namespace BugTrackingSystem.ViewModels
{
    using BugTrackingSystem.Models.Entities;
    using System.ComponentModel.DataAnnotations;

    public class ApplicationViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}

