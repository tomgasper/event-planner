using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Events
{
    public class InputEventModel
    {
        public int EventId { get; set; }

        [Required]
		[Display(Name = "Event Name")]
		[DataType(DataType.Text)]
        public string Name { get; set; }
        public SelectList? CategoryList { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Required]
		[Display(Name = "Date")]
		[DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }
        public SelectList? EventTypesList { get; set; }
        [Required]
        [Display(Name = "Event type")]
        public int EventTypeId { get; set; }
        [Required]
		[Display(Name = "Street")]
		[DataType(DataType.Text)]
        public string StreetName { get; set; }
        [Required]
		[Display(Name = "City")]
		[DataType(DataType.Text)]
        public string CityName { get; set; }
        [Required]
		[Display(Name = "Country")]
		[DataType(DataType.Text)]
        public string CountryName { get; set; }
        [Required]
		[Display(Name = "Postal Code")]
		[DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        [Required]
		[Display(Name = "Building Number")]
		[DataType(DataType.Text)]
        public string BuildingNumber { get; set; }
		[Display(Name = "Maximum number of participants")]
		public int MaxNumberParticipants { get; set; }
		[Display(Name = "Image URL")]
		[DataType(DataType.Text)]
        public string? ImageUrl { get; set; }
    }
}
