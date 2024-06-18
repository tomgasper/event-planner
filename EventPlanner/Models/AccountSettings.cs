namespace EventPlanner.Models
{
	public class AccountSettings
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public bool AccountHidden { get; set; } = false;
		public AppUser User { get; set; } = null!;
	}
}
