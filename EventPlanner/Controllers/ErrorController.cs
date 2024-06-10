using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
	public class ErrorController : Controller
	{
		public IActionResult Index(string message)
		{
			ViewBag.Message = message;
			return View();
		}
	}
}
