using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace EventPlanner.Models.Events
{
    public class EventsSearchCriteria
    {
        public string? SearchName { get; set; }
        public string? SearchCity { get; set; }
        public int? SearchDateOptionId { get; set; }
        public int? SearchCategoryId { get; set; }

        public int? SearchEventTypeId { get; set; }
    }
}
