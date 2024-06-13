namespace EventPlanner.Models
{
	public class EventsListViewModel
	{
		public IEnumerable<Event> Events { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
	}
}