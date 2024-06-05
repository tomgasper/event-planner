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
        private readonly IEventService _eventService;

        public EventsController(ILogger<EventsController> logger, IDbContext context, UserManager<AppUser> userManager, IEventService eventService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _eventService = eventService;
        }

        // Helper function for populating Category type drop down list
        private async Task PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from c in _context.Category
                                  orderby c.Name
                                  select c;

            var categories = await categoriesQuery.AsNoTracking().ToListAsync();

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategory);
        }

        public async Task<IActionResult> Index(int id)
        {
            var events = await _eventService.GetAllEventsAsync();
            return View(events);
        }

		[HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropDownList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(InputEventModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var location = await _eventService.GetOrCreateLocationAsync(model);

					var newEvent = new Event
					{
						Name = model.Name,
						CategoryId = model.CategoryId,
						DateTime = model.DateTime,
						Location = location,
						MaxNumberParticipants = model.MaxNumberParticipants,
						ImageUrl = model.ImageUrl
					};

					// Check if the same event already exists
					if (await _eventService.EventExistsAsync(newEvent))
					{
						ModelState.AddModelError(string.Empty, "An event with the same details already exists.");
						// Ensure the dropdown is populated
						await PopulateCategoriesDropDownList(model.CategoryId);
						return View(model);
					}

                    await _eventService.CreateEventAsync(newEvent);
					return RedirectToAction(nameof(Index));
				}
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the event.");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the event. Please try again.");
				}
            }

            await PopulateCategoriesDropDownList(model.CategoryId);
            return View(model);
        }
    }
}