using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Models.Events
{
	public class DateDropdownListVM
	{
		public SelectList DateOptions = new SelectList(
			new List<SelectListItem>
			{
				new SelectListItem{ Text= "Any", Value = "0" },
				new SelectListItem{ Text= "Today", Value = "1" },
				new SelectListItem{ Text= "This week", Value = "3" },
				new SelectListItem{ Text= "This month", Value = "4" }
			}
		);
	}
}