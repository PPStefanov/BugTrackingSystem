namespace BugTrackingSystem.ViewModels
{
    using BugTrackingSystem.Models.Entities;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.ComponentModel.DataAnnotations;

    public class BugReportCreateModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int PriorityId { get; set; }
        [Required]
        public int ApplicationId { get; set; }
        public string AssignedToUserId { get; set; }
        [Required]
        public int StatusId { get; set; }
    }
}

