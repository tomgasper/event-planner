using EventPlanner.Models.Events;
using EventPlanner.Models.Profile;
using EventPlanner.Models.User;

namespace EventPlanner.Interfaces
{
    public interface IProfileService
	{
		Task<IEnumerable<Event>> GetUserEvents(int userId, bool showOnlyMyEvents);
		int GetTotalPages(int eventsNo, int eventsPerPage);
		EventsListViewModel ConstructEventsListVM(int userId, IEnumerable<EventListEntryVM> paginatedEvents, int currPageNo, int totalPages, string sortCriteria, bool showOnlyMyEvents);
		Task<EventsListViewModel> GetEventsForCurrentPage(int userId, int pageNo, string sortCriteria, bool showOnlyMyEvents);
		EventListEntryVM MapEventToListEntry(Event fetchedEvent);
		IEnumerable<EventListEntryVM> MapAllEventsToVM(IEnumerable<Event> fetchedEvents);
		Task<IEnumerable<EventListEntryVM>> GetEventsForView(int userId, string sortCriteria, bool showOnlyMyEvents);
		IEnumerable<EventListEntryVM> GetEventsForPage(IEnumerable<EventListEntryVM> allEvents, int pageNo, int eventsPerPage);
		IEnumerable<EventListEntryVM> SortUserEvents(IEnumerable<EventListEntryVM> fetchedEvents, string sortCriteria);
		EventsListViewModel SetModelSortCriteria(ref EventsListViewModel model, string sortCriteriaSelected, bool showOnlyMyEvents);

		LoginHistoryVM MapToLoginHistoryVM(LoginHistory fetchedEntry);
		Task<IEnumerable<LoginHistory>> GetLoginHistories(int userId, int numberOfLastEntries);
		Task<IEnumerable<LoginHistoryVM>> GetLoginHistoriesVM(int userId, int numberOfLastEntries);
		Task<SettingsVM> GetSettingsPageVM(int userId);
		Task ToggleAccountVisibility(int userId);
		public Task DeleteUserPicture(AppUser user);

    }
}
