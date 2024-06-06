using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventsService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetEventsForPageAsync(int pageNo, int eventsPerPage);
        Task<Location> GetOrCreateLocationAsync(InputEventModel model);
        Task<bool> EventExistsAsync(Event newEvent);
        Task CreateEventAsync(Event newEvent);
    }
}
