namespace EventPlanner.Models.Profile
{
    public class SettingsVM
    {
        public bool? AccountHidden { get; set; }
        public IEnumerable<LoginHistoryVM> LoginHistory { get; set; } = null!;
    }
}
