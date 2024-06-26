﻿using EventPlanner.Exceptions;
using EventPlanner.Interfaces;
using EventPlanner.Models.Events;
using EventPlanner.Models.User;
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
		private async Task<InputEventModel> GetBlankVMWithFilledSelectLists(object? selectedCategory = null, object? selectedType = null)
        {
			var blankViewModel = new InputEventModel();
            blankViewModel.CategoryList = await _eventService.PopulateCategoriesDropDownList(selectedCategory);
            blankViewModel.EventTypesList = await _eventService.PopulateEventTypesDropDownList(selectedType);

            return blankViewModel;
		}

        public async Task<IActionResult> Index(int Id)
        {
            string userId = _userManager.GetUserId(User);
			EventViewModel retrievedEvent = await _eventService.GetEventForViewById(userId, Id);
            return View(retrievedEvent);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            InputEventModel viewModel = await GetBlankVMWithFilledSelectLists();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(InputEventModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Incorrect data in form input. Please try again.");

                var modelFilledDropdownLists = await _eventService.FillDropDownLists(model, model.CategoryId, model.EventTypeId);
                return View(modelFilledDropdownLists);
            }

            
            var user = await _userManager.GetUserAsync(User);
            Event newEvent = await _eventService.CreateEventFromInputModelAsync(user, model);

            // Check if the same event already exists
            if (await _eventService.EventExistsAsync(newEvent))
            {
                ModelState.AddModelError(string.Empty, "An event with the same details already exists.");
                // Ensure the dropdown is populated
                var modelFilledDropdownLists = await _eventService.FillDropDownLists(model, model.CategoryId, model.EventTypeId);
                return View(modelFilledDropdownLists);
            }

            var insertedEvent = await _eventService.AddEventAsync(newEvent);
            return RedirectToAction(nameof(Index), new { id = insertedEvent.Id });
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
        public async Task<IActionResult> Edit(int id, InputEventModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Incorrect data in form input. Please try again.");
                var modelFilledDropdownLists = await _eventService.FillDropDownLists(model, model.CategoryId, model.EventTypeId);
                return View(modelFilledDropdownLists);
            }

            int userId = Int32.Parse(_userManager.GetUserId(User));

            bool eventUpdated = await _eventService.UpdateEventAsync(id, userId, model);
            if (!eventUpdated) {
                return RedirectToAction("Index", "Error", new { message = "Unauthorized or event not found" });
            }

            return RedirectToAction(nameof(Index), new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string userIdString = _userManager.GetUserId(User) ?? throw new UserManagementException("User not found for delete Event operation.");
			int userId = Int32.Parse(userIdString);

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
			var userId = _userManager.GetUserId(User) ?? throw new UserManagementException("User not found when trying to assign for an event");
			await _eventService.AssignEventToUserAsync(Int32.Parse(userId), id);

			return RedirectToAction(nameof(Index), new { id = id });
		}

        [Authorize]
		[HttpGet]
		public async Task<IActionResult> UnenrollUserFromEvent(int id)
        {
            AppUser user = await _userManager.GetUserAsync(User) ?? throw new UserManagementException("User not found for removing from event's participants list.");

            await _eventService.UnenrollUserFromEvent(user, id);

            return RedirectToAction(nameof(Index), new { id });
		}
	}
}