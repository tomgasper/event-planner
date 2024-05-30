using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}