using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILogger<EventsController> _logger;
        private IDbContext _context;
        private UserManager<AppUser> _userManager;
        private readonly IEventsService _eventService;

        public EventsController(ILogger<EventsController> logger, IDbContext context, UserManager<AppUser> userManager, IEventsService eventService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(int Id = 1)
        {
            const int MAX_ENTRIES_PER_PAGE = 100;
            var events = await _eventService.GetEventsForPageAsync(Id, MAX_ENTRIES_PER_PAGE);
            return View(events);
        }
    }
}