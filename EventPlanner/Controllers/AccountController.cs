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
        private ILoginHistoryService _loginHistoryService { get; }

        public AccountController(UserManager<AppUser> userManager, IAccountService accountService, ILoginHistoryService loginHistoryService)
        {
            _userManager = userManager;
            _accountService = accountService;
            _loginHistoryService = loginHistoryService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Image,UserName,Email,FirstName,LastName,Password,ConfirmPassword")]InputUserModel inputModel)
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

                // Auto login user
                bool loginSuccess = await _accountService.LoginWithLog(inputModel.UserName, inputModel.Password, HttpContext.Connection.RemoteIpAddress?.ToString());

                if (loginSuccess) return RedirectToAction("Index", "Home");
                else
                {
                    RedirectToAction(nameof(Login));
                }

            }

            return View(inputModel);
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
            bool loginSuccess = await _accountService.LoginWithLog(inputModel.UserName, inputModel.Password, HttpContext.Connection.RemoteIpAddress?.ToString());

            if (loginSuccess) { return LocalRedirect("/"); }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }

		[Authorize]
		[HttpGet]
		public IActionResult Delete()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Delete")]
		public async Task<IActionResult> DeletePost()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "User not found");
				return View("NotFound");
			}

			var result = await _userManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				await _accountService.Logout();
				return RedirectToAction("Index", "Home");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View();
		}
	}
}
