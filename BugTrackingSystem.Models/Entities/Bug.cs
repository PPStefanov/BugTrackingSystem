using System;
using System.ComponentModel.DataAnnotations;
using BugTrackingSystem.Models.Enums;

namespace BugTrackingSystem.Models.Entities
{
    public class Bug
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public BugStatus Status { get; set; } = BugStatus.Open;

        public BugPriority Priority { get; set; } = BugPriority.Medium;

        // You can add other properties as needed
    }
}