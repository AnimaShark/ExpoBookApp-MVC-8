using ExpoBookApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpoBookApp.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;

        public VenueController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Leaser")]
        [HttpGet]
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var venues = _context.Venues
                .Include(v => v.CreatedBy)
                .Where(v => v.CreatedBy.Email == userEmail)
                .ToList();
            return View(venues);
        }

        [Authorize(Roles = "Leaser")]
        [HttpGet]
        // GET: Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venues venue)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user == null) return Unauthorized();

            venue.CreatedByUserId = user.Id;
            venue.CreatedAt = DateTime.UtcNow;
            venue.ApprovalStatus = ApprovalStatus.Pending;

            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venue/Edit
        [Authorize(Roles = "Leaser")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @venue = await _context.Venues.FindAsync(id);
            if (@venue == null)
                return NotFound();

            if (venue.ApprovalStatus == ApprovalStatus.Approved)
            {
                ModelState.AddModelError("", "Approved venues cannot be edited.");
                return RedirectToAction(nameof(Index));
            }

            return View(@venue);
        }

        // POST: Venue/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venues venue)
        {
            var userEmail = User.Identity.Name; // assuming email is stored in cookie
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (id != venue.Id)
                return NotFound();

            // Check if the venue name is already taken by another venue created by the same user
            if (_context.Venues.Any(v =>
                                    v.Name == venue.Name &&
                                    v.CreatedByUserId == user.Id &&
                                    v.Id != venue.Id))
            {
                ModelState.AddModelError("Name", "Venue name already exists for another venue created by you.");
                return View(venue);
            }

            //Maintain CreatedByUserId to original user
            if (user == null)
                return Unauthorized();
            venue.CreatedByUserId = user.Id; 

            //Maintain CreatedAt value to original
            venue.CreatedAt = _context.Venues.AsNoTracking().FirstOrDefault(v => v.Id == id)?.CreatedAt ?? DateTime.UtcNow;


            venue.ApprovalStatus = ApprovalStatus.Pending;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venues.Any(v => v.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venue/Details
        [Authorize(Roles = "Leaser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // GET: Venue/Delete
        [Authorize(Roles = "Leaser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venue/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
