namespace EventPlanner.Models
{
	public class SettingsVM
	{
		public bool? AccountHidden { get; set; }
		public IEnumerable<LoginHistoryVM> LoginHistory { get; set; } = null!;
	}
}
