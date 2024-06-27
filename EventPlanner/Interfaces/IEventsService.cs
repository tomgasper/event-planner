using EventPlanner.Models.Events;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Interfaces
{
    public interface IEventsService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetEventsRangeAsync(int noEntries = 100);
        Task<IQueryable<Event>> BuildEventQuery(EventsSearchCriteria criteria);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<EventType>> GetEventTypesAsync();

        EventsSearchCriteria MapInputToCriteria(EventsViewModel viewModel);
        Task<EventsViewModel> SearchEvents(EventsViewModel inputViewModel);
        SelectList CreateCategorySelectList(IEnumerable<Category> categories, int? searchCategoryId);
        SelectList CreateTypeSelectList(IEnumerable<EventType> types, int? searchTypeId);
		Task<EventsViewModel> GetEventsForIndex();
	}
}
