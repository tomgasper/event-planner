namespace EventPlanner.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int Category { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; } = null!;
        public int MaxNumberParticipants { get; set; }
        public string? ImageUrl { get; set; }
    }
}
