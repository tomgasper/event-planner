using EventPlanner.Interfaces;
using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IDbContext _context;
        private readonly UserManager<AppUser> _userManager;

		public EventController(IEventService eventService, IDbContext context, UserManager<AppUser> userManager) {
            _context = context;
            _userManager = userManager;
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

        [Authorize]
        public async Task<IActionResult> AssignEventToUser(int id)
        {
            AppUser user = await _context.Users.Include(u => u.Events).FirstOrDefaultAsync();
            Event? eventById = _context.Event.FirstOrDefault( e => e.Id == id);

            if (eventById == null)
            {
                // Event not found
                return RedirectToAction("Index", "Error", new { message = "User not found" });
			}

            if (user.Events != null && user.Events.Any(e => e.Id == id))
            {
				// User already assigned to the event
				return RedirectToAction("Index", "Error", new { message = "Event not found" });
			}

            user.Events.Add(eventById);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id=id } );
        }

        public async Task<IActionResult> Error()
        {
            return View();
        }

    }
}