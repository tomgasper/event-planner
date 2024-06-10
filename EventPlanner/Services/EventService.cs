using EventPlanner.Models;
using EventPlanner.Interfaces;
using System.Linq;
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
        public async Task<EventViewModel> GetEventForViewById(int id)
        {
            var result = await _context.Event
                .Include("Location").Include("Location.Street").Include("Location.Street.City").Include("Location.Street.City.Country")
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                EventViewModel model = new();
                model.Id = result.Id;
                model.Name = result.Name;
                model.Category = null;

                var lookupCategory = await _context.Category.FirstOrDefaultAsync(c => c.Id == result.CategoryId);

                if (lookupCategory != null)
                {
                    model.Category = lookupCategory.Name;
                }

                // Address is deeply nested in relational db
                if (result.Location != null)
                {
                    model.PostalCode = result.Location.PostalCode;
                    model.BuidingNo = result.Location.BuildingNumber;
                    model.Street = null;
                    model.City = null;
                    model.Country = null;

                    if (result.Location.Street != null)
                    {
                        model.Street = result.Location.Street.Name;

                        if (result.Location.Street.City != null)
                        {
                            model.City = result.Location.Street.City.Name;

                            if (result.Location.Street.City.Country != null)
                            {
                                model.Country = result.Location.Street.City.Country.Name;
                            }
                        }
                    }
                }
                return model;
            }
            return null;
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

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            _context.Event.Add(newEvent);
            await _context.SaveChangesAsync();

            return newEvent;
        }
    }
}
