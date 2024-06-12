using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace EventPlanner.Models
{
	public class EventsViewModel
	{
		public string SearchName { get; set; }
		public string SearchCity { get; set; }
		public DateTime? SearchDate { get; set; }
		public SelectList? SearchCategories { get; set; }
		public int? SearchCategoryId { get; set; }
		public SelectList? SearchEventTypes { get; set; }
		public int? SearchEventTypeId { get; set; }
		public IEnumerable<Event> Events { get; set; }
	}
}
