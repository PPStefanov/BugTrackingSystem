﻿namespace BugTrackingSystem.ViewModels.BugReport
{
    using BugTrackingSystem.Models.Entities;
    using System.ComponentModel.DataAnnotations;


        public class BugReportListViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Status { get; set; }
            public string Priority { get; set; }
            public string Application { get; set; }
            public string AssignedTo { get; set; }
            public string Reporter { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? LastEditDate { get; set; }
            public string LastEditUser { get; set; } = string.Empty;
            public bool CanEdit { get; set; }
        }

}

