using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
	public class ErrorController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
