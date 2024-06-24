using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Models.User
{
    public class AppUserRole : IdentityRole<int>
    {
        public AppUserRole(string Name) : base(Name)
        {

        }
    }
}
