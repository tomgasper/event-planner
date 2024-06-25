namespace EventPlanner.Models.Events
{
    public class EventListEntryVM
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
        public string EventTypeName { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateTime { get; set; }
    }
}
