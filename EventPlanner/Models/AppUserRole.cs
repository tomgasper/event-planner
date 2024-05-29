using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Models
{
    public class AppUserRole : IdentityRole<int>
    {
        public AppUserRole(string Name) : base(Name)
        {

        }
    }
}
