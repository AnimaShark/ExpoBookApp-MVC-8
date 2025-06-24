using Microsoft.AspNetCore.Mvc;

namespace ExpoBookApp.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
