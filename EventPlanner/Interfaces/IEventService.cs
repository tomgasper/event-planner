using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Location> GetOrCreateLocationAsync(InputEventModel model);
        Task<bool> EventExistsAsync(Event newEvent);
        Task CreateEventAsync(Event newEvent);
    }
}
