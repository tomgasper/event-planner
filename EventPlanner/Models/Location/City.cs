namespace EventPlanner.Models.Location
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Street> Streets { get; set; }
    }
}