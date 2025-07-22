using BugTrackingSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackingSystem.ViewModels.Reports
{
    public class ReportFilterOptionsViewModel
    {
        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Priorities { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Applications { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Reporters { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Developers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ReportTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GroupByOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();
    }

    public class ReportTemplateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ReportType ReportType { get; set; }
        public string FilterJson { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDefault { get; set; }
    }
}