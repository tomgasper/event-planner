using EventPlanner.Interfaces;
using EventPlanner.Models;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService) {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(int Id)
        {
            // Get the correct event id
            // Fetch from the db via service
            // Display via View Model
            // Happy

            EventViewModel retrievedEvent = await _eventService.GetEventForViewById(Id);
            return View(retrievedEvent);
        }
    }
}