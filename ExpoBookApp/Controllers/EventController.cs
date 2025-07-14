using ExpoBookApp.Migrations;
using ExpoBookApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpoBookApp.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: /Event/Index
        public IActionResult Index(string typeFilter = null)
        {
            var userEmail = User.Identity?.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            //Convert UTC to Malaysia Time Zone
            var malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            var now = DateTime.UtcNow;


            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }

            var vm = new EventIndexViewModel();

            // Only show events created by the organizer
            vm.CreatedEvents = _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => e.CreatedBy.Email == userEmail && !e.IsCancelled)
                .ToList();

            

            // Only show upcomming events created by the organizer
            vm.CreatedUpcomingEvents = _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => e.CreatedBy.Email == userEmail && e.StartDate > now && !e.IsCancelled)
                .OrderBy(e => e.StartDate)
                .ToList();
            
            // List of upcoming events
            vm.UpcomingEvents = _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => e.StartDate > now && !e.IsCancelled)
                .OrderBy(e => e.StartDate)
                .Take(5)
                .ToList();

            // List of all events (optionally filtered by theme)
            var allEventsQuery = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(typeFilter))
            {
                allEventsQuery = allEventsQuery
                    .Where(e => e.EventType == typeFilter && !e.IsCancelled);
            }
            vm.AllEvents = allEventsQuery
                .Include(e => e.CreatedBy)
                .ToList();

            // Event Type for dropdown filter
            vm.EventType = _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => !e.IsCancelled)
                .Select(e => e.EventType)
                .Distinct()
                .ToList();

            vm.TypeFilter = typeFilter;

            return View(vm);
        }

        //GET: /Event/Explore
        [Authorize(Roles = "Participants")]
        public IActionResult Explore(string search, DateTime? startDate, DateTime? endDate)
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var events = _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => e.StartDate >= DateTime.UtcNow);

            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e =>
                    e.EventName.Contains(search) ||
                    e.Description.Contains(search));
            }

            if (startDate.HasValue)
            {
                events = events.Where(e => e.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                events = events.Where(e => e.EndDate <= endDate.Value);
            }

            var eventList = events.OrderBy(e => e.StartDate)
                .ToList();

            return View(eventList);
        }

        // GET: /Event/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (@event == null)
                return NotFound();

            return View(@event);
        }

        [Authorize]
        // GET: /Event/Create
        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venues
                .Include(v => v.CreatedBy)
                .Where(v => v.ApprovalStatus == ApprovalStatus.Approved && v.IsActive)
                .ToList();

            return View();
        }

        // POST: /Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event, IFormFile ImageFile)
        {
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                var userEmail = User.Identity.Name; // assuming email is stored in cookie
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user == null) return Unauthorized();

                var today = DateTime.UtcNow;

                @event.CreatedByUserId = user.Id; // Set the creator of the event

                //Check if the event name is already taken
                if (_context.Events.Any(e => e.EventName == @event.EventName && e.CreatedByUserId == user.Id))
                {
                    ModelState.AddModelError("EventName", "An event with this name already exists.");
                    return View(@event);
                }

                //Date validation
                //Check if end date is not before start date
                if (@event.EndDate < @event.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be after the start date.");
                    return View(@event);
                }

                //Check if the event is in the past
                if (@event.StartDate < today)
                {
                    ModelState.AddModelError("StartDate", "Start date must be after today.");
                    return View(@event);
                }

                //Free event to mark as public event
                if (@event.TicketPrice == 0 && @event.TicketQuota > 0)
                {
                    @event.IsPublic = true;
                }

                ////Image check and save
                //if (ImageFile != null && ImageFile.Length > 0)
                //{
                //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                //    if (!Directory.Exists(uploadsFolder))
                //        Directory.CreateDirectory(uploadsFolder);

                //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await ImageFile.CopyToAsync(stream);
                //    }

                //    @event.ImagePath = "/images/" + uniqueFileName;
                //}

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Venues = _context.Venues
                .Where(v => v.ApprovalStatus == ApprovalStatus.Approved && v.IsActive)
                .ToList();

                return View(@event);
            }
        }

        [Authorize]
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
            var userEmail = User.Identity.Name; // assuming email is stored in cookie
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            var today = DateTime.UtcNow;

            @event.CreatedByUserId = user.Id;

            //Check if the event name is already taken
            if (_context.Events.Any(e => 
                                    e.EventName == @event.EventName && 
                                    e.CreatedByUserId == user.Id &&
                                    e.Id != @event.Id))
            {
                ModelState.AddModelError("EventName", "An event with this name already exists.");
                return View(@event);
            }

            //Date validation
            if (@event.EndDate < @event.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date.");
                return View(@event);
            }
            if (@event.StartDate < today)
            {
                ModelState.AddModelError("StartDate", "Start date must be after today.");
                return View(@event);
            }


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

        [Authorize]
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
                @event.IsCancelled = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
