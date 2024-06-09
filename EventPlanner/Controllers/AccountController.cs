using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using EventPlanner.Interfaces;

namespace EventPlanner.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private IAccountService _accountService { get; }

        public AccountController(UserManager<AppUser> userManager, IAccountService accountService)
        {
            _userManager = userManager;
            _accountService = accountService; 
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
        public async Task<IActionResult> Index(InputEditUserModel inputModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                await _accountService.EditUserInfo(inputModel, user);
			}

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(InputUserModel inputModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateNewUser(inputModel);
                if (!result.Item1.Succeeded)
                {
                    foreach (var error in result.Item1.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return LocalRedirect("/Home");
                }

                IdentityResult addToRoleAsync = await _accountService.AddToRoleAsync(result.Item2, "Member");

                if (!addToRoleAsync.Succeeded)
                {
                    // Log error message
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Login()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();   
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(InputLoginModel inputModel)
        {
            var result = await _accountService.Login(inputModel);
            if (result.Succeeded)
            {
                ViewBag.Result = "success";
                return LocalRedirect("/");
            } else
            {
                ViewBag.Result = "fail";
            }

            return View();
        }

        public async Task<IActionResult> ManageEvents()
        {
            return View();
        }

    }
}
