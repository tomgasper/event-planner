using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using EventPlanner.Data;

namespace EventPlanner.Controllers
{
    public class AccountController : Controller
    {
        private EventPlannerDbContext _context;
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EventPlannerDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            InputEditUserModel inputModel = new ();
            inputModel.UserName = user.UserName;
            inputModel.Email = user.Email;
            inputModel.FirstName = user.FirstName;
            inputModel.LastName = user.LastName;

            return View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InputEditUserModel inputModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.UserName = inputModel.UserName;
                user.Email = inputModel.Email;
                user.FirstName = inputModel.FirstName;
                user.LastName = inputModel.LastName;

                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(InputUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }

                    return LocalRedirect("/Home");
                }

                IdentityResult addToRoleAsync = await _userManager.AddToRoleAsync(user, "Member");

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
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(InputLoginModel inputModel)
        {
            var result = await _signInManager.PasswordSignInAsync(
                inputModel.UserName, inputModel.Password, false, false);

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

    }
}
