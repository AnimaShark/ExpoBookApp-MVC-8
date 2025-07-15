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
            var userEmail = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized("User not found");

            var selectedEvent = _context.Events
                //.Include(e => e.Venue)
                .Where(e => !e.IsCancelled)
                .FirstOrDefault(e => e.Id == eventId);

            if (selectedEvent == null)
                return NotFound();

            // Check if user already bought tickets
            var existingTicket = _context.Tickets
                .Where(t => t.EventId == eventId && t.UserId == user.Id)
                .Sum(t => t.Quantity);

            int alreadyBought = existingTicket;
            int remaining = Math.Max(0, 5 - alreadyBought);

            var vm = new BuyTicketViewModel
            {
                Event = selectedEvent,
                AlreadyBought = alreadyBought,
                MaxRemaining = remaining
            };

            return View(vm);
        }

        // POST: Ticket/ConfirmTicket
        [HttpPost]
        public async Task<IActionResult> ConfirmTicket(int eventId, int TicketQty)
        {
            var userEmail = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized();

            var selectedEvent = _context.Events.FirstOrDefault(e => e.Id == eventId);

            if (selectedEvent == null || selectedEvent.IsCancelled)
                return NotFound("Event not available");

            // Get how many tickets this user has already bought for this event
            var ticketsAlreadyBought = _context.Tickets
                .Where(t => t.UserId == user.Id && t.EventId == eventId)
                .Sum(t => t.Quantity);

            // Enforce per-user limit for non-public events
            if (!selectedEvent.IsPublic && ticketsAlreadyBought + TicketQty > 5)
            {
                TempData["ErrorMessage"] = $"You've already purchased {ticketsAlreadyBought} ticket(s). " +
                                           $"You can only buy up to 5 in total for this event.";
                return RedirectToAction("BuyTicket", new { eventId });
            }

            // Enforce overall event quota
            if (selectedEvent.TicketQuota > 0 && selectedEvent.TicketBought + TicketQty > selectedEvent.TicketQuota)
            {
                TempData["ErrorMessage"] = "Sorry, this event is already sold out.";
                return RedirectToAction("Explore", "Event");
            }

            // Ticket code generation logic
            var today = DateTime.UtcNow.Date;
            var datePrefix = today.ToString("yyMMdd");
            var todayTicketsCount = _context.Tickets.Count(t => t.PurchaseDate.Date == today);
            var ticketCode = $"{datePrefix}-{todayTicketsCount.ToString("D4")}";

            selectedEvent.TicketBought += TicketQty;

            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = user.Id,
                Quantity = TicketQty,
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
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ticket purchase failed for user {UserEmail} on event {EventId}", userEmail, eventId);
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
