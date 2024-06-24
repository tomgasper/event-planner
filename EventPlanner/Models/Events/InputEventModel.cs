using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Events
{
    public class InputEventModel
    {
        public int EventId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public SelectList? CategoryList { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }
        public SelectList? EventTypesList { get; set; }
        [Required]
        [Display(Name = "Event type")]
        public int EventTypeId { get; set; }
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
