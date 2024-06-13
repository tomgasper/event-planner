using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
	public interface IProfileService
	{
		Task<IEnumerable<Event>> GetUserEvents(int userId);
		int GetTotalPages(int eventsNo, int eventsPerPage);
		EventsListViewModel ConstructEventsListVM(IEnumerable<Event> paginatedEvents, int currPageNo, int totalPages);
		Task<EventsListViewModel> GetEventsForCurrentPage(int userId, int pageNo);
	}
}
