using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventService
    {
        Task<EventViewModel> GetEventForViewById(int id);
        Task<bool> EventExistsAsync(Event newEvent);
        Task<Event> AddEventAsync(Event newEvent);
        Task<Event> CreateEventFromInputModelAsync(InputEventModel model);
        Task<Location> GetOrCreateLocationAsync(InputEventModel model);
        Task<InputEventModel> GetEventForEdit(Event? fetchedEvent);
        Task<Event?> GetFullEventAsync(int id);
        Task<IEnumerable<Category>> GetListOfCategories();
        Task<bool> UpdateEventAsync(int eventId, int userId, InputEventModel updatedModel);
        bool UserHasPermissionToEdit(int userId, int authorId);
    }
}
