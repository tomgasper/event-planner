using EventPlanner.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EventPlanner.Models
{
    public class Event
    {
        public int Id { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public string Name { get; set; } = null!;
		[ForeignKey("Category")]
		public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
		[ForeignKey("EventType")]
		public int EventTypeId { get; set; }
		public EventType EventType { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public Location Location { get; set; } = null!;
		public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
		public int MaxNumberParticipants { get; set; }
        public string? ImageUrl { get; set; }
    }
}
