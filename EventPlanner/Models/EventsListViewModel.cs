using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Models
{
	public class EventsListViewModel
	{
		public SelectList? SortCriteria { get; set; }
		public string SortCriteriaSelected { get; set; }
		public IEnumerable<EventListEntryVM> Events { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public bool ShowOnlyMyEvents { get; set; }
	}
}