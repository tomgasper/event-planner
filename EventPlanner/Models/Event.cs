using EventPlanner.Models;

namespace EventPlanner.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateTime { get; set; }
        public Location Location { get; set; }
        public int MaxNumberParticipants { get; set; }
        public string? ImageUrl { get; set; }
    }
}
