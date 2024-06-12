using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

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
				SearchDate = DateTime.Today,
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

		public async Task<IQueryable<Event>> BuildEventQuery(EventsSearchCriteria criteria)
		{
			var eventsQuery = _context.Event.AsQueryable();

			if (!String.IsNullOrEmpty(criteria.SearchName))
			{
				eventsQuery = eventsQuery.Where(s => s.Name!.Contains(criteria.SearchName));
			}

			if (!String.IsNullOrEmpty(criteria.SearchCity))
			{
				eventsQuery = eventsQuery.Where(e => e.Location.Street.City.Name == criteria.SearchCity);
			}

			// To do:
			// Add useful range for dates like: any time, today, tomorrow, this week, next week
			/*
			if (criteria.SearchDate.HasValue)
			{
				eventsQuery = eventsQuery.Where(e => e.DateTime.Date == criteria.SearchDate.Value);
			}
			*/

			if (criteria.SearchCategoryId.HasValue)
			{
				eventsQuery = eventsQuery.Where(e => e.CategoryId == criteria.SearchCategoryId.Value );
			}

			if (criteria.SearchCategoryId.HasValue && await _context.Category.AnyAsync(c => c.Id == criteria.SearchCategoryId.Value) )
			{
				eventsQuery = eventsQuery.Where(e => e.CategoryId == criteria.SearchCategoryId.Value);
			}

			if (criteria.SearchEventTypeId.HasValue &&
				await _context.EventType.AnyAsync(t => t.Id == criteria.SearchEventTypeId.Value))
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
				SearchDate = viewModel.SearchDate,
				SearchCategoryId = viewModel.SearchCategoryId,
				SearchEventTypeId = viewModel.SearchEventTypeId,
			};

			return criteria;
		}

		public async Task<EventsViewModel> SearchEvents(EventsViewModel inputViewModel)
		{
			EventsSearchCriteria criteria = MapInputToCriteria(inputViewModel);
			var eventsQuery = await BuildEventQuery(criteria);
			var types = await GetEventTypesAsync();
			var categories = await GetCategoriesAsync();

			var viewModel = new EventsViewModel
			{
				SearchName = criteria.SearchName,
				SearchCity = criteria.SearchCity,
				SearchDate = criteria.SearchDate,
				SearchCategories = new SelectList(categories, "Id", "Name", criteria.SearchCategoryId),
				SearchEventTypes = new SelectList(types, "Id", "Name", criteria.SearchEventTypeId),
				Events = await eventsQuery.ToListAsync()
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
