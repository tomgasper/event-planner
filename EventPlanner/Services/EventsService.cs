using EventPlanner.Data;
using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using EventPlanner.Models.Events;

namespace EventPlanner.Services
{
    public class EventsService : IEventsService
	{
		private readonly IDbContext _context;

		public EventsService(IDbContext context)
		{ 
			_context = context;
		}

		public async Task<List<Event>> GetAllEventsAsync()
		{
			return await _context.Event.AsNoTracking().ToListAsync();
		}

		public async Task<EventsViewModel> GetEventsForIndex()
		{
			const int MAX_EVENTS_START_PAGE = 100;

			List<Event> events = await GetEventsRangeAsync(MAX_EVENTS_START_PAGE);
			IEnumerable<Category> categories = await GetCategoriesAsync();
			IEnumerable<EventType> types = await GetEventTypesAsync();

			var viewModel = new EventsViewModel()
			{
				Events = events,
				SearchCategories = new SelectList(categories, "Id", "Name"),
				SearchEventTypes = new SelectList(types, "Id", "Name"),
			};

			return viewModel;
		}

		public async Task<List<Event>> GetEventsRangeAsync(int noEntries = 100)
		{
			if (noEntries >= _context.Event.Count())
			{
				return await _context.Event.AsNoTracking().ToListAsync();
			}

			return await _context.Event.Skip(0).Take(noEntries).AsNoTracking().ToListAsync();
		}

		public IQueryable<Event> BuildEventQuery(EventsSearchCriteria criteria)
		{
			var eventsQuery = _context.Event.Include(e => e.Author).AsQueryable();

			if (!String.IsNullOrEmpty(criteria.SearchName))
			{
				eventsQuery = eventsQuery.Where(s => s.Name!.Contains(criteria.SearchName));
			}

			if (!String.IsNullOrEmpty(criteria.SearchCity))
			{
				eventsQuery = eventsQuery.Where(e => e.Location.Street.City.Name == criteria.SearchCity);
			}
			
			if (criteria.SearchDateOptionId.HasValue)
			{
				switch (criteria.SearchDateOptionId)
				{
					case 0:
						break;
					case 1:
						eventsQuery = eventsQuery.Where(e => e.DateTime.Date == DateTime.Today);
						break;
					case 2:
						var dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
						eventsQuery = eventsQuery.Where(e => e.DateTime.Date >= dt && e.DateTime.Date <= dt.AddDays(6));
						break;
					case 3:
						var now = DateTime.Now;
						var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
						var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
						eventsQuery = eventsQuery.Where(e => e.DateTime.Date >= firstDayOfMonth && e.DateTime.Date <= lastDayOfMonth);
						break;
				}
			}

			if (criteria.SearchCategoryId.HasValue && criteria.SearchCategoryId > 0)
			{
				eventsQuery = eventsQuery.Where(e => e.CategoryId == criteria.SearchCategoryId.Value );
			}

			if (criteria.SearchEventTypeId.HasValue && criteria.SearchEventTypeId > 0)
			{
				eventsQuery = eventsQuery.Where(e => e.EventTypeId == criteria.SearchEventTypeId.Value);
			}

			return eventsQuery;
		}

		public EventsSearchCriteria MapInputToCriteria(EventsViewModel viewModel)
		{
			var criteria = new EventsSearchCriteria()
			{
				SearchName = viewModel.SearchName,
				SearchCity = viewModel.SearchCity,
				SearchDateOptionId = viewModel.SearchDate.SelectedOption,
				SearchCategoryId = viewModel.SearchCategoryId,
				SearchEventTypeId = viewModel.SearchEventTypeId,
			};

			return criteria;
		}

		public SelectList CreateCategorySelectList(IEnumerable<Category> categories, int? searchCategoryId)
		{
			return new SelectList(
			new[] { new Category { Id = 0, Name = "Any" } }.Concat(categories),
			"Id", "Name", searchCategoryId);
		}

		public SelectList CreateTypeSelectList(IEnumerable<EventType> types, int? searchTypeId)
		{
			return new SelectList(
			new[] { new EventType { Id = 0, Name = "Any" } }.Concat(types),
			"Id", "Name", searchTypeId);
		}

		public async Task<EventsViewModel> SearchEvents(EventsViewModel inputViewModel)
		{
			EventsSearchCriteria criteria = MapInputToCriteria(inputViewModel);
			var eventsQuery = BuildEventQuery(criteria);
			var types = await GetEventTypesAsync();
			var categories = await GetCategoriesAsync();

			var viewModel = new EventsViewModel
			{
				SearchName = criteria.SearchName,
				SearchCity = criteria.SearchCity,
				SearchDate = inputViewModel.SearchDate,
				SearchCategories = CreateCategorySelectList(categories,criteria.SearchCategoryId),
				SearchEventTypes = CreateTypeSelectList(types, criteria.SearchEventTypeId),
				Events = await eventsQuery.Include(e => e.Author).ToListAsync()
			};

			return viewModel;
		}

		public async Task<IEnumerable<Category>> GetCategoriesAsync()
		{
			return await (from c in _context.Category
					orderby c.Name
					select c).Distinct().ToListAsync();
		}

		public async Task<IEnumerable<EventType>> GetEventTypesAsync()
		{
			return await (from et in _context.EventType
						  orderby et.Name
						  select et).Distinct().ToListAsync();
		}
	}
}
