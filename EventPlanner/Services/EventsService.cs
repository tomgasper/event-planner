using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;

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


		public async Task<List<Event>> GetEventsForPageAsync(int pageNo, int eventsPerPage = 8)
		{
			if (pageNo < 0) { throw new InvalidOperationException("Page number must be 1 or higher"); }

			int skipEntries = (pageNo - 1) * eventsPerPage;
			return await _context.Event.Skip(skipEntries).Take(eventsPerPage).AsNoTracking().ToListAsync();
		}
	}
}
