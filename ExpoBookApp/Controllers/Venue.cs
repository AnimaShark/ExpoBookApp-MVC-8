using Microsoft.AspNetCore.Mvc;

namespace ExpoBookApp.Controllers
{
    public class VenueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
