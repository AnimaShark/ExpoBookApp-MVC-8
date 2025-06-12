using Microsoft.AspNetCore.Mvc;
using ExpoBookApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpoBookApp.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Event/
        public IActionResult Index(string themeFilter = null)
        {
            var today = DateTime.Today;

            // Upcoming events - those that haven't started yet
            var upcomingEvents = _context.Events
                .Where(e => e.StartDate >= today)
                .OrderBy(e => e.StartDate)
                .Take(5) // Optional: Limit to 5
                .ToList();

            // All events (optionally filtered by theme)
            var allEvents = string.IsNullOrEmpty(themeFilter)
                ? _context.Events.ToList()
                : _context.Events.Where(e => e.Theme == themeFilter).ToList();

            // Unique theme list for filtering
            var themes = _context.Events
                .Select(e => e.Theme)
                .Distinct()
                .ToList();

            var viewModel = new EventIndexViewModel
            {
                UpcomingEvents = upcomingEvents,
                AllEvents = allEvents,
                ThemeFilter = themeFilter,
                Themes = themes
            };

            return View(viewModel);
        }

        // GET: /Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // GET: /Event/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: /Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // POST: /Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: /Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // POST: /Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
