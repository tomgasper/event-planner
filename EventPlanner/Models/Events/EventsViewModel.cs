using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace EventPlanner.Models.Events
{
    public class EventsViewModel
    {
		[Display(Name = "Event Name")]
		public string SearchName { get; set; }
		[Display(Name = "City")]
		public string SearchCity { get; set; }
        [DataType(DataType.Date)]
		[Display(Name = "City")]
		public DateTime? SearchDate { get; set; }
		[Display(Name = "Category")]
		public SelectList? SearchCategories { get; set; }
        public int? SearchCategoryId { get; set; }
		[Display(Name = "Type")]
		public SelectList? SearchEventTypes { get; set; }
        public int? SearchEventTypeId { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}
