using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Utils
{
    public class Utils
    {
        
        static public async Task CreateRoles(IServiceProvider serviceProvider, ConfigurationManager config)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppUserRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            foreach (string roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new AppUserRole(roleName));
                }
            }

            // Create Admin account if it doesn't exist already
            var superUser = new AppUser
            {
                UserName = config["AppSettings:UserName"],
                Email = config["AppSettings:UserName"]
            };

            string userPWD = config["AppSettings:UserPassword"];
            var _user = await userManager.FindByEmailAsync(config["AppSettings:AdminUserEmail"]);

            if (_user == null)
            {
                var createSuperUser = await userManager.CreateAsync(superUser, userPWD);
                if (createSuperUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, "Admin");
                }
            }
        }
    }
}
