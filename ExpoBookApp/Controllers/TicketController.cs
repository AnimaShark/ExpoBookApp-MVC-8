using Microsoft.AspNetCore.Mvc;

namespace ExpoBookApp.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BuyTicket()
        {
            return View();
        }
    }
}
