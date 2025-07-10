using ExpoBookApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpoBookApp.Controllers
{
    [Authorize(Roles = "Participants")]
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TicketController> _logger;

        public TicketController(AppDbContext context, ILogger<TicketController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ticket/BuyTicket
        [HttpGet]
        public IActionResult BuyTicket(int eventId)
        {
            var selectedEvent = _context.Events
                //.Include(e => e.Venue)
                .Where(e => !e.IsCancelled)
                .FirstOrDefault(e => e.Id == eventId);

            if (selectedEvent == null)
                return NotFound();

            return View("BuyTicket", selectedEvent);
        }

        // POST: Ticket/ConfirmTicket
        [HttpPost]
        public async Task<IActionResult> ConfirmTicket(int eventId, int TicketQty)
        {
            var userEmail = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            // Ticket code generation logic
            var today = DateTime.UtcNow.Date;
            var datePrefix = today.ToString("yyMMdd");

            var todayTicketsCount = _context.Tickets
                .Count(t => t.PurchaseDate.Date == today);

            var nextTicketNumber = todayTicketsCount + 1;

            var ticketCode = $"{datePrefix}-{todayTicketsCount.ToString("D4")}"; // Format: yyMMdd-0001

            var ticketQuantity = TicketQty;

            if (user == null)
                return Unauthorized();
            
            // Assign Ticket values
            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = user.Id,
                Quantity = ticketQuantity,
                PurchaseDate = DateTime.UtcNow,
                TicketCode = ticketCode
            };

            _context.Tickets.Add(ticket);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ticket purchased successfully!";
                return RedirectToAction("MyTickets");
            }
            catch (DbUpdateException ex) //Handle concurrency issues or other database update exceptions
            {
                _logger.LogError(ex, "Ticket purchase failed for user {UserEmail} on event {EventId}", userEmail, eventId); //log the error

                TempData["ErrorMessage"] = "Ticket purchase failed due to high traffic. Please try again.";
                return RedirectToAction("BuyTicket", new { eventId });
            }
        }

        // GET: Ticket/MyTickets
        public IActionResult MyTickets()
        {
            var userEmail = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized();

            var malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");

            var tickets = _context.Tickets
                .Include(t => t.Event)
                .Where(t => t.UserId == user.Id)
                .AsEnumerable() // switch to client-side projection to allow null checks
                .Select(t => new TicketViewModel
                {
                    TicketCode = t.TicketCode,
                    EventName = t.Event?.EventName,
                    Venue = t.Event?.Venue,
                    StartDate = t.Event != null
                        ? TimeZoneInfo.ConvertTimeFromUtc(t.Event.StartDate, malaysiaTimeZone)
                        : (DateTime?)null,
                    Quantity = t.Quantity,
                    PurchaseDate = TimeZoneInfo.ConvertTimeFromUtc(t.PurchaseDate, malaysiaTimeZone),
                    IsCancelled = t.Event?.IsCancelled ?? false,
                    IsEventMissing = t.Event == null
                })
                .ToList();

            return View(tickets);
        }

    }
}
