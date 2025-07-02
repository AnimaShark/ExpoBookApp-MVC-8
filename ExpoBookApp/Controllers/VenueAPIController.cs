using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpoBookApp.Models; // adjust if needed
using System.Linq;

namespace ExpoBookApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VenuesApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/venuesapi/approved
        [HttpGet("approved")]
        public IActionResult GetApprovedVenues()
        {
            var venues = _context.Venues
                .Where(v => v.ApprovalStatus == ApprovalStatus.Approved)
                .Select(v => new {
                    v.Id,
                    v.Name,
                    v.Size,
                    v.Address,
                    Capacity = (int)((v.Size / 60)*8), //Assumeing 60 sq ft per 8 person
                    CreatedBy = v.CreatedBy.Email
                })
                .ToList();

            return Ok(venues);
        }
    }
}
