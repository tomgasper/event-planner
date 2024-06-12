﻿using EventPlanner.Models;
using System.Security.Claims;

namespace EventPlanner.Interfaces
{
    public interface IEventService
    {
        Task<EventViewModel> GetEventForViewById(int id);
        Task<bool> EventExistsAsync(Event newEvent);
        Task<Event> AddEventAsync(Event newEvent);
        Task<Event> CreateEventFromInputModelAsync(AppUser user, InputEventModel model);
        Task<Location> GetOrCreateLocationAsync(InputEventModel model);
        Task<InputEventModel> GetEventForEdit(Event? fetchedEvent);
        Task<Event?> GetFullEventAsync(int id);
        Task<IEnumerable<Category>> GetListOfCategories();
        Task<bool> UpdateEventAsync(int eventId, int userId, InputEventModel updatedModel);
        bool UserHasPermissionToEdit(ClaimsPrincipal user, int userId, int authorId);

		Task<bool> DeleteEventAsync(ClaimsPrincipal user, int eventId, int userId);
        Task<bool> AssignEventToUserAsync(int userId, int eventId);
	}
}
