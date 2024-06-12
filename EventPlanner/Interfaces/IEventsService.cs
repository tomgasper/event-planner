using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventsService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetEventsRangeAsync(int noEntries);
        Task<IQueryable<Event>> BuildEventQuery(EventsSearchCriteria criteria);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<EventType>> GetEventTypesAsync();

        EventsSearchCriteria MapInputToCriteria(EventsViewModel viewModel);
        Task<EventsViewModel> SearchEvents(EventsViewModel inputViewModel);

        Task<EventsViewModel> GetEventsForIndex();
	}
}
