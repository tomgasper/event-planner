using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}