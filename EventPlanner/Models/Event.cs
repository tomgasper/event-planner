using EventPlanner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Event
    {
        public int Id { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public AppUser Author { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateTime { get; set; }
        public Location Location { get; set; }
		public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
		public int MaxNumberParticipants { get; set; }
        public string? ImageUrl { get; set; }
    }
}
