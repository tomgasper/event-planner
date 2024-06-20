using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILogger<EventsController> _logger;
        private IDbContext _context;
        private UserManager<AppUser> _userManager;
        private readonly IEventsService _eventsService;

        public EventsController(ILogger<EventsController> logger, IDbContext context, UserManager<AppUser> userManager, IEventsService eventsService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _eventsService = eventsService;
        }

        public async Task<IActionResult> Index()
        {
			try
            {
                EventsViewModel eventsAndDefaulSearchInfo = await _eventsService.GetEventsForIndex();
				return View(eventsAndDefaulSearchInfo);

			} catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = "Error occured while processing your request. Please try again!" });
            }
        }

		public async Task<IActionResult> Search(EventsViewModel input)
        {
            try
            {
				var viewModel = await _eventsService.SearchEvents(input);
				return View(viewModel);
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Error", new { message = "Error occured while processing your request. Please try again!" });
			}
        }
    }
}