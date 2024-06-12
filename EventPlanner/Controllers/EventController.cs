﻿using EventPlanner.Interfaces;
using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Helper function for populating Category type drop down list
        private async Task<InputEventModel> PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            IEnumerable<Category> categories = await _eventService.GetListOfCategories();
            InputEventModel viewModel = new InputEventModel();
            viewModel.CategoryList = new SelectList(categories, "Id", "Name", selectedCategory);

            return viewModel;
        }

        public async Task<IActionResult> Index(int Id)
        {
            EventViewModel retrievedEvent = await _eventService.GetEventForViewById(Id);
            return View(retrievedEvent);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var viewModel = await PopulateCategoriesDropDownList();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(InputEventModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Incorrect data in form input. Please try again.");
                await PopulateCategoriesDropDownList(model.CategoryId);
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                Event newEvent = await _eventService.CreateEventFromInputModelAsync(user, model);

                // Check if the same event already exists
                if (await _eventService.EventExistsAsync(newEvent))
                {
                    ModelState.AddModelError(string.Empty, "An event with the same details already exists.");
                    // Ensure the dropdown is populated
                    await PopulateCategoriesDropDownList(model.CategoryId);
                    return View(model);
                }

                var insertedEvent = await _eventService.AddEventAsync(newEvent);
                return RedirectToAction(nameof(Index), new { id = insertedEvent.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the event. Please try again.");
                await PopulateCategoriesDropDownList(model.CategoryId);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            // Check whether the logged in user = author
            int userId = Int32.Parse(_userManager.GetUserId(User));
            var fetchedEvent = await _eventService.GetFullEventAsync(id);

            if (fetchedEvent == null || !_eventService.UserHasPermissionToEdit(User, userId, fetchedEvent.AuthorId) )
            {
				return RedirectToAction("Index", "Error", new { message = "You don't have permission to edit this entry." });
            }

            InputEventModel viewModel = await _eventService.GetEventForEdit(fetchedEvent);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, InputEventModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Incorrect data in form input. Please try again.");
                await PopulateCategoriesDropDownList(viewModel.CategoryId);
                return View(viewModel);
            }

            int userId = Int32.Parse(_userManager.GetUserId(User));

            bool eventUpdated = await _eventService.UpdateEventAsync(id, userId, viewModel);
            if (!eventUpdated) {
                return RedirectToAction("Index", "Error", new { message = "Unauthorized or event not found" });
            }

            return RedirectToAction(nameof(Index), new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
			int userId = Int32.Parse(_userManager.GetUserId(User));

            bool deleteSuccesful = await _eventService.DeleteEventAsync(User, id, userId);

			if (!deleteSuccesful)
			{
				return RedirectToAction("Index", "Error", new { message = "You don't have permission to delete this entry." });
			}

			return RedirectToAction("Index", "Events");
        }

        [Authorize]
        public async Task<IActionResult> AssignEventToUser(int id)
        {
			var userId = Int32.Parse(_userManager.GetUserId(User));
			bool success = await _eventService.AssignEventToUserAsync(userId, id);

			if (!success)
			{
				return RedirectToAction("Index", "Error", new { message = "Operation failed or unauthorized" });
			}

			return RedirectToAction(nameof(Index), new { id = id });
		}

        public async Task<IActionResult> Error()
        {
            return View();
        }

    }
}