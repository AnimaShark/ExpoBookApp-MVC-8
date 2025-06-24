using ExpoBookApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExpoBookApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; 
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BrowseEvent(string typeFilter = null)
        {
            var today = DateTime.Today;

            // Upcoming events - those that haven't started yet
            var upcomingEvents = _context.Events
                .Where(e => e.StartDate >= today)
                .OrderBy(e => e.StartDate)
                .Take(5) // Optional: Limit to 5
                .ToList();

            // All events (optionally filtered by theme)
            var allEvents = string.IsNullOrEmpty(typeFilter)
                ? _context.Events.ToList()
                : _context.Events.Where(e => e.EventType == typeFilter).ToList();

            // Unique theme list for filtering
            var eventType = _context.Events
                .Select(e => e.EventType)
                .Distinct()
                .ToList();

            var viewModel = new EventIndexViewModel
            {
                UpcomingEvents = upcomingEvents,
                AllEvents = allEvents,
                TypeFilter = typeFilter,
                EventType = eventType
            };

            return View(viewModel);
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
