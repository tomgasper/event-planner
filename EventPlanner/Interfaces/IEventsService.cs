using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventsService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetEventsForPageAsync(int pageNo, int eventsPerPage);
    }
}
