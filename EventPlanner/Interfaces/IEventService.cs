using EventPlanner.Models;

namespace EventPlanner.Interfaces
{
    public interface IEventService
    {
        Task<EventViewModel> GetEventForViewById(int id);
    }
}
