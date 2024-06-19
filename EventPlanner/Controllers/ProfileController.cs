using EventPlanner.Interfaces;
using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventPlanner.Controllers
{
    public class ProfileController : Controller
    {
        private ILogger<ProfileController> _logger { get; }
        private UserManager<AppUser> _userManager { get; }
        private IAccountService _accountService { get; }

        private IProfileService _profileService { get; }

        public ProfileController(UserManager<AppUser> userManager, IAccountService accountService, ILogger<ProfileController> logger, IProfileService profileService)
        {
            _logger = logger;
            _userManager = userManager;
            _accountService = accountService;
            _profileService = profileService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            InputEditUserModel inputModel = _accountService.PassInputUserInfo(user);
            return View(inputModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(InputEditUserModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(nameof(Index), model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _accountService.EditUserInfo(model, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user profile");
                ModelState.AddModelError(string.Empty, "An unexpected error occured.");
                return View(nameof(Index), model);
            }

            return View(nameof(Index),model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ManageEvents(int id = 1, string sortCriteriaSelected = "", bool showOnlyMyEvents = false)
        {
            var model = new EventsListViewModel();
            model = _profileService.SetModelSortCriteria(ref model, sortCriteriaSelected, showOnlyMyEvents);

			try
            {
				int userId = Int32.Parse(_userManager.GetUserId(User));
				model = await _profileService.GetEventsForCurrentPage(userId, id, sortCriteriaSelected, showOnlyMyEvents);
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to get users events.");
				ModelState.AddModelError(string.Empty, "An unexpected error occured.");
                return View(nameof(Index), model);
			}

			return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User) ?? throw new Exception("Couldn't retrieve user");
                SettingsVM loginHistoryVM = await _profileService.GetSettingsPageVM(user.Id);
                return View(loginHistoryVM);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user login history");
                ModelState.AddModelError(string.Empty, "An unexpected error occured while fetching login history.");
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> HideProfile()
        {
            // Get logged in user
            // Change user.accountSettings to true
            // In another class filter out hidden profiles (event/attendending)

            try
            {
				var user = await _userManager.GetUserAsync(User);
				await _profileService.ToggleAccountVisibility(user.Id);
				
			} catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle account visibility");
				return RedirectToAction("Index", "Error", new { message = "Unauthorized or user not found" });
			}

			return RedirectToAction(nameof(Settings));
		}

        
    }
}
