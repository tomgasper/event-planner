using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Services
{
    public class IdentitySetupService
    {
        private readonly RoleManager<AppUserRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public IdentitySetupService(RoleManager<AppUserRole> roleManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task CreateRolesAsync()
        {
            // Add Roles to the Identity System
            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            foreach (string roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new AppUserRole(roleName));
                }
            }
        }

        public async Task CreateAdmin()
        {
            var superUser = new AppUser
            {
                UserName = _configuration["AppSettings:UserName"],
                Email = _configuration["AppSettings:UserName"]
            };

            string userPWD = _configuration["AppSettings:UserPassword"];
            var _user = await _userManager.FindByEmailAsync(_configuration["AppSettings:AdminUserEmail"]);

            if (_user == null)
            {
                var createSuperUser = await _userManager.CreateAsync(superUser, userPWD);
                if (createSuperUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(superUser, "Admin");
                }
            }
        }
    }
}
