using EventPlanner.Data;
using EventPlanner.Models;
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
        private EventPlannerDbContext _context;
        private UserManager<AppUser> _userManager;

        public EventsController(ILogger<EventsController> logger, EventPlannerDbContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // Helper function for populating drop down list
        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from c in _context.Category
                                  orderby c.Name
                                  select c;
            ViewBag.CategoryId = new SelectList(categoriesQuery.AsNoTracking(), "Id", "Name", selectedCategory);
        }

        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            PopulateCategoriesDropDownList();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(InputEventModel model)
        {
            if (ModelState.IsValid)
            {
                // Address constructed in a relational way so need to go through a lot of queries
                var country = await _context.Country.FirstOrDefaultAsync(c => c.Name == model.CountryName);
                if (country == null)
                {
                    country = new Country { Name = model.CountryName };
                    _context.Country.Add(country);
                }

                var city = await _context.City.FirstOrDefaultAsync(c => c.Name == model.CityName && c.Country.Id == country.Id);
                if (city == null)
                {
                    city = new City { Name = model.CityName, Country = country };
                    _context.City.Add(city);
                }

                var street = await _context.Street
                    .FirstOrDefaultAsync(s => s.Name == model.StreetName && s.City.Id == city.Id);
                if (street == null)
                {
                    street = new Street { Name = model.StreetName, City = city };
                    _context.Street.Add(street);
                }

                var location = await _context.Location.FirstOrDefaultAsync(
                    l => l.Street == street && l.PostalCode == model.PostalCode && l.BuildingNumber == model.BuildingNumber
                    );

                if (location == null)
                {
                    location = new Location
                    {
                        Street = street,
                        PostalCode = model.PostalCode,
                        BuildingNumber = model.BuildingNumber
                    };
                    _context.Add(location);
                }

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
                if (_context.Event.Any(e =>
                e.Name == newEvent.Name && e.DateTime == newEvent.DateTime && e.Location == newEvent.Location
                ))
                {
                    _context.ChangeTracker.Clear();
                    ModelState.AddModelError(string.Empty, "An event with the same details already exists.");
                    // Ensure the dropdown is populated
                    PopulateCategoriesDropDownList(model.CategoryId);
                    return View(model);
                }

                _context.Event.Add(newEvent);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateCategoriesDropDownList(model.CategoryId);
            return View(model);
        }
    }
}