using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventPlanner.Services
{
	public class ProfileService : IProfileService
	{
		private IDbContext _context { get; }
		private IEventsService _eventsService { get; }
		public ProfileService(IDbContext context, IEventsService eventsService)
		{
			_context = context;
			_eventsService = eventsService;
		}

		public async Task<IEnumerable<Event>> GetUserEvents(int userId)
		{
			var retrivedUser = await _context.Users
				.Include(u => u.Events)
					.ThenInclude( e => e.Location)
						.ThenInclude( l => l.Street )
							.ThenInclude(s => s.City)
				.Include(u => u.Events)
					.ThenInclude(e => e.Author)
				.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

			return retrivedUser.Events;
		}

		public int GetTotalPages(int eventsNo, int eventsPerPage)
		{
			return (int)Math.Ceiling(eventsNo / (double)eventsPerPage);
		}

		public IEnumerable<Event> GetEventsForPage(IEnumerable<Event> allEvents, int pageNo, int eventsPerPage)
		{
			return allEvents.Skip((pageNo - 1) * eventsPerPage).Take(eventsPerPage);
		}

		public EventsListViewModel ConstructEventsListVM(IEnumerable<Event> paginatedEvents, int currPageNo, int totalPages)
		{
			var model = new EventsListViewModel()
			{
				Events = paginatedEvents,
				CurrentPage = currPageNo,
				TotalPages = totalPages
			};

			return model;
		}

		public async Task<EventsListViewModel> GetEventsForCurrentPage(int userId, int pageNo)
		{
			const int EVENTS_PER_PAGE = 5;

			IEnumerable<Event> events = await GetUserEvents(userId);
			int totalEvents = events.Count();
			int totalPages = GetTotalPages(totalEvents, EVENTS_PER_PAGE);
			IEnumerable<Event> paginatedEvents = GetEventsForPage(events, pageNo, EVENTS_PER_PAGE);

			return ConstructEventsListVM(paginatedEvents, pageNo, totalPages);
		}
	}
}
