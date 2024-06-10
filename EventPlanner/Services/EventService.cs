using EventPlanner.Models;
using EventPlanner.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Services
{
    public class EventService : IEventService
    {
        private readonly IDbContext _context;
        
        public EventService(IDbContext context)
        {
            _context = context;
        }

        public bool UserHasPermissionToEdit(int userId, int authorId)
        {
            if (userId != authorId) return false;
            else return true;
        }

        public async Task<bool> UserAssignedToEvent(AppUser user, Event fetchedEvent)
        {
            return await _context.Event.AnyAsync(e => e.Id == fetchedEvent.Id && e.Users.Any(u => u.Id == user.Id));
        }

        public async Task AssignUserToEvent(AppUser user, Event fetchedEvent)
        {
            user.Events.Add(fetchedEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateEventAsync(int eventId, int userId, InputEventModel updatedModel)
        {
            var eventToUpdate = await GetFullEventAsync(eventId);
            if (eventToUpdate == null || eventToUpdate.AuthorId != userId)
            {
                return false;
            }

            eventToUpdate.Name = updatedModel.Name;
            eventToUpdate.CategoryId = updatedModel.CategoryId;
            eventToUpdate.DateTime = updatedModel.DateTime;
            eventToUpdate.Location = await GetOrCreateLocationAsync(updatedModel);
            eventToUpdate.MaxNumberParticipants = updatedModel.MaxNumberParticipants;
            eventToUpdate.ImageUrl = updatedModel.ImageUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetListOfCategories()
        {
            var categoriesQuery = from c in _context.Category
                                  orderby c.Name
                                  select c;

            IEnumerable<Category> categories = await categoriesQuery.AsNoTracking().ToListAsync();

            return categories;
        }

        public async Task<LocationViewModel> GetEventLocationAsync(Event fetchedEvent)
        {
            LocationViewModel model = new LocationViewModel();
            model.PostalCode = fetchedEvent.Location.PostalCode;
            model.BuildingNumber = fetchedEvent.Location.BuildingNumber;
            model.StreetName = null;
            model.CityName = null;
            model.CountryName = null;

            if (fetchedEvent.Location.Street != null)
            {
                model.StreetName = fetchedEvent.Location.Street.Name;

                if (fetchedEvent.Location.Street.City != null)
                {
                    model.CityName = fetchedEvent.Location.Street.City.Name;

                    if (fetchedEvent.Location.Street.City.Country != null)
                    {
                        model.CountryName = fetchedEvent.Location.Street.City.Country.Name;
                    }
                }
            }

            return model;
        }

        public async Task<Event?> GetFullEventAsync(int id)
        {
            var result = await _context.Event
                .Include("Location").Include("Location.Street").Include("Location.Street.City").Include("Location.Street.City.Country")
                .FirstOrDefaultAsync(e => e.Id == id);

            return result;
        }

        public async Task<EventViewModel> GetEventForViewById(int id)
        {
            var result = await GetFullEventAsync(id);

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
                    LocationViewModel location = await GetEventLocationAsync(result);

                    model.PostalCode = result.Location.PostalCode;
                    model.BuidingNo = result.Location.BuildingNumber;
                    model.Street = location.StreetName;
                    model.City = location.CityName;
                    model.Country = location.CountryName;
                }
                return model;
            }
            return null;
        }

        public async Task<InputEventModel> GetEventForEdit(Event? fetchedEvent)
        {
            if (fetchedEvent != null)
            {
                InputEventModel model = new();
                model.EventId = fetchedEvent.Id;
                model.Name = fetchedEvent.Name;
                model.CategoryId = fetchedEvent.CategoryId;

                // Address is deeply nested in relational db
                if (fetchedEvent.Location != null)
                {
                    LocationViewModel location = await GetEventLocationAsync(fetchedEvent);

                    model.PostalCode = fetchedEvent.Location.PostalCode;
                    model.BuildingNumber = fetchedEvent.Location.BuildingNumber;
                    model.StreetName = location.StreetName;
                    model.CityName = location.CityName;
                    model.CountryName = location.CountryName;
                }

                IEnumerable<Category> categories = await GetListOfCategories();
                model.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);

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

        public async Task<Event> AddEventAsync(Event newEvent)
        {
            _context.Event.Add(newEvent);
            await _context.SaveChangesAsync();

            return newEvent;
        }

        public async Task<Event> CreateEventFromInputModelAsync(InputEventModel model)
        {
            var location = await GetOrCreateLocationAsync(model);

            var newEvent = new Event
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                DateTime = model.DateTime,
                Location = location,
                MaxNumberParticipants = model.MaxNumberParticipants,
                ImageUrl = model.ImageUrl
            };

            return newEvent;
        }
    }
}
