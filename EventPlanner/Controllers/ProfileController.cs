﻿using EventPlanner.Interfaces;
using EventPlanner.Models;
using EventPlanner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EventPlanner.Controllers
{
    public class ProfileController : Controller
    {
        private ILogger<ProfileController> _logger { get; }
        private UserManager<AppUser> _userManager { get; }
        private IAccountService _accountService { get; }

        private IProfileService _profileService { get; }

        private IDbContext _context { get; }

        public ProfileController(UserManager<AppUser> userManager, IAccountService accountService, ILogger<ProfileController> logger, IDbContext context, IProfileService profileService)
        {
            _logger = logger;
            _context = context;
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
        public async Task<IActionResult> ManageEvents(int id = 1)
        {
            var model = new EventsListViewModel();

            try
            {
				int userId = Int32.Parse(_userManager.GetUserId(User));
				model = await _profileService.GetEventsForCurrentPage(userId, id);
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to get users events.");
				ModelState.AddModelError(string.Empty, "An unexpected error occured.");
                return View(nameof(Index), model);
			}

			return View(model);
        }

        public async Task<IActionResult> Settings()
        {
            return View();
        }
    }
}