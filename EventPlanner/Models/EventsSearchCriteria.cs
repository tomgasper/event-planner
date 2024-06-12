using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace EventPlanner.Models
{
	public class EventsSearchCriteria
	{
		public string SearchName { get; set; }
		public string SearchCity { get; set; }
		public DateTime? SearchDate { get; set; }
		public int? SearchCategoryId { get; set; }

		public int? SearchEventTypeId { get; set; }
	}
}
