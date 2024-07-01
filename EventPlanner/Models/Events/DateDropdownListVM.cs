using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Models.Events
{
	public class DateDropdownListVM
	{
		public int? SelectedOption { get; set; }

		public SelectList Options = new SelectList(
			new List<SelectListItem>
			{
				new SelectListItem{ Text="Any", Value ="0" },
				new SelectListItem{ Text="Today", Value ="1" },
				new SelectListItem{ Text="This week", Value ="2" },
				new SelectListItem{ Text="This month", Value ="3" }
			}, "Value", "Text"
		);
	}
}