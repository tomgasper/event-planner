using EventPlanner.Models;
using EventPlanner.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

using EventPlanner.Models.User;
using EventPlanner.Models.Events;
using EventPlanner.Models.Location;
using System.Data;

using System;

using EventPlanner.Exceptions;

namespace EventPlanner.Services
{
    public class EventService : IEventService
    {
        private readonly IDbContext _context;
        
        public EventService(IDbContext context)
        {
            _context = context;
        }

        public bool UserHasPermissionToEdit(ClaimsPrincipal user, int userId, int authorId)
        {
            if (user.IsInRole("Admin")) { return true; }
            return userId == authorId;
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

		// To do add option for unassigning user from event
        public async Task UnenrollUserFromEvent(AppUser user, int eventId)
        {
            _context.Attach(user);
            await _context.Entry(user).Collection(u => u.Events).LoadAsync();

			Event? fetchedEvent = await _context.Event.FirstOrDefaultAsync((e) => e.Id == eventId);
            if ( fetchedEvent == null )
            {
				throw new NotFoundException("Event you are trying to delete couldn't be retrieved");
			}
            
            user.Events.Remove(fetchedEvent);
            await _context.SaveChangesAsync();
        }

		public async Task AssignEventToUserAsync(int userId, int eventId)
		{
			var user = await _context.Users.Include(u => u.Events).FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
			{
                throw new UserManagementException("Couldn't retrieve the current user");
			}

            if (user.Events.Any(e => e.Id == eventId))
            {
                throw new InvalidInputException("Current user is already assigned to this event.");
            }

			var eventToAssign = await _context.Event.FirstOrDefaultAsync(e => e.Id == eventId);
			if (eventToAssign == null)
			{
                throw new NotFoundException("Couldn't retrieve the current event");
			}

			user.Events.Add(eventToAssign);
			_context.Update(user);
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

        public async Task<bool> DeleteEventAsync(ClaimsPrincipal user, int eventId, int userId)
        {
            var fetchedEvent = _context.Event.FirstOrDefault(e => e.Id == eventId);

			if (fetchedEvent == null || !UserHasPermissionToEdit(user, userId, fetchedEvent.AuthorId))
            {
                return false;
            }

            _context.Event.Remove(fetchedEvent);
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

        public async Task<IEnumerable<EventType>> GetListOfEventTypes()
        {
            var eventTypeQuery = from et in _context.EventType
                                  orderby et.Name
                                  select et;

            IEnumerable<EventType> types = await eventTypeQuery.AsNoTracking().ToListAsync();

            return types;
        }

        public LocationViewModel GetEventLocation(Event fetchedEvent)
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
                .Include(e => e.Users)
                    .ThenInclude(u => u.AccountSettings)
                .Include(e => e.Author)
                .Include(e => e.Location)
                    .ThenInclude(l => l.Street)
                        .ThenInclude(s => s.City)
                            .ThenInclude(ci => ci.Country)
                .FirstOrDefaultAsync(e => e.Id == id);

            return result;
        }

        public IEnumerable<AppUser> GetNotHiddenUsers(IEnumerable<AppUser> userList)
        {
            return userList.Where(u => (u.AccountSettings == null) || (u.AccountSettings.AccountHidden == false) );
        }

        public async Task<EventViewModel> GetEventForViewById(string? userId, int id)
        {
            var result = await GetFullEventAsync(id);

            int? userIdInt = null;
            if (userId != null)
            {
                userIdInt = Int32.Parse(userId);
            }

            if (result != null)
            {
                EventViewModel model = new();
                model.Id = result.Id;
                model.Author = result.Author;
                model.DateTime = result.DateTime;
                model.UserId = userIdInt;
                model.Name = result.Name;
                model.IsUserAuthor = userId == (result.AuthorId.ToString());
                model.Category = null;
                model.UsersAttending = result.Users;

                var lookupCategory = await _context.Category.FirstOrDefaultAsync(c => c.Id == result.CategoryId);

                if (lookupCategory != null)
                {
                    model.Category = lookupCategory.Name;
                }

                // Address is deeply nested in relational db
                if (result.Location != null)
                {
                    LocationViewModel location = GetEventLocation(result);

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

        public async Task<InputEventModel?> GetEventForEdit(Event? fetchedEvent)
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
                    LocationViewModel location = GetEventLocation(fetchedEvent);

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

        public async Task<Event> CreateEventFromInputModelAsync(AppUser user, InputEventModel model)
        {
            // Attach user to context if it's not already tracked
            var attachedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id) ?? user;
            if (_context.Entry(attachedUser).State == EntityState.Detached)
			{
				_context.Users.Attach(attachedUser);
			}

			var location = await GetOrCreateLocationAsync(model);

            var newEvent = new Event
            {
                Author = attachedUser,
                Name = model.Name,
                CategoryId = model.CategoryId,
                DateTime = model.DateTime,
                Location = location,
                EventTypeId = model.EventTypeId,
                MaxNumberParticipants = model.MaxNumberParticipants,
                ImageUrl = model.ImageUrl
            };

            return newEvent;
        }

		public async Task<SelectList> PopulateCategoriesDropDownList(object? selectedCategory = null)
		{
			IEnumerable<Category> categories = await GetListOfCategories();
			var categoryList = new SelectList(categories, "Id", "Name", selectedCategory);

			return categoryList;
		}

        public async Task<SelectList> PopulateEventTypesDropDownList(object? selectedType = null)
        {
            IEnumerable<EventType> types = await GetListOfEventTypes();
            var eventTypesList = new SelectList(types, "Id", "Name", selectedType);

            return eventTypesList;
        }

        public async Task<InputEventModel> FillDropDownLists(InputEventModel viewModel, object selectedCategory, object selectedType)
        {
            viewModel.CategoryList = await PopulateCategoriesDropDownList(selectedCategory);
            viewModel.EventTypesList = await PopulateEventTypesDropDownList(selectedType);

            return viewModel;
        }
    }
}
