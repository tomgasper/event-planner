using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class InputEventModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string StreetName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string CityName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string CountryName { get; set; }
        [Required]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string BuildingNumber { get; set; }
        public int MaxNumberParticipants { get; set; }
        [DataType(DataType.Text)]
        public string? ImageUrl { get; set; }
    }
}
