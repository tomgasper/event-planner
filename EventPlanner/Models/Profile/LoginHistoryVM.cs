using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Profile
{
    public class LoginHistoryVM
    {
        [Display(Name = "Login Time")]
        public DateTime? LoginTime { get; set; }
		[DisplayName("IP Address")]
		public string? IPAddress { get; set; }
		[DisplayName("Device")]
		public string? DeviceInformation { get; set; }
		[DisplayName("Browser")]
		public string? BrowserInformation { get; set; }
		[DisplayName("Login Succeeded")]
		public bool? LoginSuccess { get; set; }
		[DisplayName("Failure Reason")]
		public string? FailureReason { get; set; }
    }
}
