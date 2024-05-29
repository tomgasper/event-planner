using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using EventPlanner.Models;

namespace EventPlanner.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            var user = new AppUser
            {
                UserName = "bola",
                Email = "user@email.com"
            };

            var result = await _userManager.CreateAsync(user, "haslo123");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return LocalRedirect("/Home");
            }

            await _userManager.AddToRoleAsync(user, "Guest");

            return RedirectToAction(nameof(Index));
        }
    }
}
