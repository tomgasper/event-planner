namespace EventPlanner.Models
{
    public class Street
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual City City { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
    }
}
