using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventService
    {
        Task<EventViewModel> GetEventForViewById(int id);
        Task<bool> EventExistsAsync(Event newEvent);
        Task<Event> CreateEventAsync(Event newEvent);
        Task<Location> GetOrCreateLocationAsync(InputEventModel model);
    }
}
