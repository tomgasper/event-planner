using EventPlanner.Models.User;

namespace EventPlanner.Models.Profile
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public DateTime LoginTime { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInformation { get; set; }
        public string BrowserInformation { get; set; }
        public bool LoginSuccess { get; set; }
        public string? FailureReason { get; set; }
        public virtual AppUser User { get; set; }
    }
}