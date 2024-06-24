using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using EventPlanner.Interfaces;
using EventPlanner.Exceptions;
using EventPlanner.Models.User;
using EventPlanner.Models.Profile;
using EventPlanner.Models.Events;

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
        [ActionName("Index")]
        public async Task<IActionResult> UpdateProfile(InputEditUserModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User) ?? throw new NotFoundException("User not found for profile update operation.");
            await _accountService.EditUserInfo(model, user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ManageEvents(int id = 1, string sortCriteriaSelected = "", bool showOnlyMyEvents = false)
        {
            var model = new EventsListViewModel();
            model = _profileService.SetModelSortCriteria(ref model, sortCriteriaSelected, showOnlyMyEvents);

			int userId = Int32.Parse(_userManager.GetUserId(User));
			model = await _profileService.GetEventsForCurrentPage(userId, id, sortCriteriaSelected, showOnlyMyEvents);

			return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User) ?? throw new NotFoundException("User not found for profile settings display.");
            SettingsVM loginHistoryVM = await _profileService.GetSettingsPageVM(user.Id);
            return View(loginHistoryVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> HideProfile()
        {
            var user = await _userManager.GetUserAsync(User) ?? throw new NotFoundException("User not found for toggle hide profile operation.");

            await _profileService.ToggleAccountVisibility(user.Id);
            
	        return RedirectToAction(nameof(Settings));
		}
    }
}
