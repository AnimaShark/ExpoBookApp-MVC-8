using ExpoBookApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    //GET: Admin/Index
    // Admin dashboard
    public IActionResult Index()
    {
        return View();
    }

    // GET: Admin/PendingVenues
    // View all venues pending approval
    public IActionResult PendingVenues()
    {
        var venues = _context.Venues
            .Include(v => v.CreatedBy)
            .Where(v => v.ApprovalStatus == ApprovalStatus.Pending)
            .ToList();
    
        return View(venues);
    }

    // GET: Admin/ApprovedVenues
    // Approve a venue
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveVenue(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue == null) return NotFound();

        venue.ApprovalStatus = ApprovalStatus.Approved;
        _context.Update(venue);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(PendingVenues));
    }

    // GET: Admin/RejectedVenues
    // Reject a venue
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectVenue(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue == null) return NotFound();

        venue.ApprovalStatus = ApprovalStatus.Rejected;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(PendingVenues));
    }

    // GET: Admin/PendingRegistrations
    public IActionResult PendingRegistrations()
    {
        var users = _context.Users
            .Where(u => !u.IsEmailConfirmed && (u.Role == "Leaser" || u.Role == "Organizer"))
            .ToList();
        return View(users);
    }
}
