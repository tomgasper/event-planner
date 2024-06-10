using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class LocationViewModel
    {
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
    }
}
