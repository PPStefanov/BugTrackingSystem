using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.ViewModels.BugReport
{
    public class BugReportFilterViewModel
    {
        [Display(Name = "Search")]
        public string SearchTerm { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string StatusFilter { get; set; } = string.Empty;

        [Display(Name = "Priority")]
        public string PriorityFilter { get; set; } = string.Empty;

        [Display(Name = "Application")]
        public string ApplicationFilter { get; set; } = string.Empty;

        [Display(Name = "Assigned To")]
        public string AssignedToFilter { get; set; } = string.Empty;

        [Display(Name = "Reporter")]
        public string ReporterFilter { get; set; } = string.Empty;

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "CreatedDate";

        [Display(Name = "Sort Order")]
        public string SortOrder { get; set; } = "desc";

        // Dropdown options
        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> PriorityOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ApplicationOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AssignedToOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ReporterOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SortByOptions { get; set; } = new List<SelectListItem>();
    }
}