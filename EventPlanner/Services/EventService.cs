using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Services
{
    public class EventService : IEventService
	{
		private readonly IDbContext _context;

		public EventService(IDbContext context)
		{ 
			_context = context;
		}

		public async Task<List<Event>> GetAllEventsAsync()
		{
			return await _context.Event.AsNoTracking().ToListAsync();
		}

		public async Task<Location> GetOrCreateLocationAsync(InputEventModel model)
		{
			// Address constructed in a relational way so need to go through a lot of queries
			var country = await _context.Country.FirstOrDefaultAsync(c => c.Name == model.CountryName);
			if (country == null)
			{
				country = new Country { Name = model.CountryName };
				_context.Country.Add(country);
			}

			var city = await _context.City.FirstOrDefaultAsync(c => c.Name == model.CityName && c.Country.Id == country.Id);
			if (city == null)
			{
				city = new City { Name = model.CityName, Country = country };
				_context.City.Add(city);
			}

			var street = await _context.Street
				.FirstOrDefaultAsync(s => s.Name == model.StreetName && s.City.Id == city.Id);
			if (street == null)
			{
				street = new Street { Name = model.StreetName, City = city };
				_context.Street.Add(street);
			}

			var location = await _context.Location.FirstOrDefaultAsync(
				l => l.Street == street && l.PostalCode == model.PostalCode && l.BuildingNumber == model.BuildingNumber
				);

			if (location == null)
			{
				location = new Location
				{
					Street = street,
					PostalCode = model.PostalCode,
					BuildingNumber = model.BuildingNumber
				};
				_context.Add(location);
			}

			return location;
		}

		public async Task<bool> EventExistsAsync(Event newEvent)
		{
			return await _context.Event.AnyAsync(e =>
				e.Name == newEvent.Name &&
				e.DateTime == newEvent.DateTime &&
				e.Location.Street.Id == newEvent.Location.Street.Id &&
				e.Location.PostalCode == newEvent.Location.PostalCode &&
				e.Location.BuildingNumber == newEvent.Location.BuildingNumber);
		}

		public async Task CreateEventAsync(Event newEvent)
		{
			_context.Event.Add(newEvent);
			await _context.SaveChangesAsync();
		}
	}
}
