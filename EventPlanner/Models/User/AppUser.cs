using EventPlanner.Models.Events;
using EventPlanner.Models.Profile;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Models.User
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Event> AuthoredEvents { get; set; } = new List<Event>();
        public AccountSettings? AccountSettings { get; set; }
    }
}