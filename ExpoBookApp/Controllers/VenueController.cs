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
        private readonly IWebHostEnvironment _hostEnvironment;

        public VenueController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = "Leaser")]
        [HttpGet]
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var venues = _context.Venues
                .Include(v => v.CreatedBy)
                .Where(v => v.CreatedBy.Email == userEmail && v.IsActive)
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
        public async Task<IActionResult> Create(Venues venue/*, IFormFile SupportDoc*/)
        { 
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user == null) return Unauthorized();

            venue.CreatedByUserId = user.Id;
            venue.CreatedAt = DateTime.UtcNow;
            venue.ApprovalStatus = ApprovalStatus.Pending;
            venue.SupportingDocumentPath = "n/a"; // Default value, Placeholder to be updated

            if (ModelState.IsValid)
            {
                ////Supporting document upload
                //if (SupportDoc != null && SupportDoc.Length > 0)
                //{
                //    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "supporting-documents");
                //    if (!Directory.Exists(uploadsFolder))
                //        Directory.CreateDirectory(uploadsFolder);

                //    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(SupportDoc.FileName)}";
                //    var filePath = Path.Combine(uploadsFolder, fileName);

                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await SupportDoc.CopyToAsync(stream);
                //    }

                //    venue.SupportingDocumentPath = "/uplaods/" +  fileName;
                //}

                _context.Add(venue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Venue created successfully and is pending approval.";
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
            venue.SupportingDocumentPath = "n/a";

            venue.ApprovalStatus = ApprovalStatus.Pending; //Set status to pending when editing to be approved again

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

        // GET: Venue/Detail
        [Authorize(Roles = "Leaser")]
        public async Task<IActionResult> Detail(int? id)
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
                .Include(v => v.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venue == null)
                return NotFound();

            var hasUpcomingEvents = _context.Events
                .Any(e => e.Venue == venue.Name && e.EndDate >= DateTime.UtcNow && !e.IsCancelled);

            ViewBag.HasUpcomingEvents = hasUpcomingEvents;

            return View(venue);
        }

        // POST: Venue/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            var affectedEvents = _context.Events
                .Where(e => e.Venue == venue.Name && e.EndDate >= DateTime.UtcNow && !e.IsCancelled)
                .ToList();

            if (venue != null)
            {
                venue.IsActive = false;
                affectedEvents.ForEach(e => e.IsCancelled = true);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
