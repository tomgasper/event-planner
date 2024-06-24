using EventPlanner.Models.User;

namespace EventPlanner.Models.Profile
{
    public class AccountSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool AccountHidden { get; set; } = false;
        public AppUser User { get; set; } = null!;
    }
}
