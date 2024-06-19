namespace EventPlanner.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public bool IsUserAuthor { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime DateTime { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string BuidingNo { get; set; }
        public int MaxNumberParticipants { get; set; }
        public string? ImageUrl { get; set; }
        public IEnumerable<AppUser> UsersAttending { get; set; }
    }
}