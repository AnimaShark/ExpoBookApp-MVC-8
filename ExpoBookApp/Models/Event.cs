using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpoBookApp.Services;

namespace ExpoBookApp.Models
{
    public class Event
    {
        /// Event Attributes
        public int Id { get; set; }
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        public string EventName { get; set; }
        public string EventType { get; set; }
        [StringLength(500, ErrorMessage = "Maximum 500 characters")]
        public string? Description { get; set; } = null;
        public string Venue { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(EventValidator), nameof(EventValidator.ValidateEndDate))]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Ticket price cannot be negative."), DataType(DataType.Currency)]
        public decimal TicketPrice { get; set; }
        public int TicketBought { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Ticket quota cannot be negative.")]
        public int TicketQuota { get; set; }


        //Image for the event
        public string? ImagePath { get; set; }

        //MARKING
        //Event is public or private
        public bool IsPublic { get; set; } = false;
        //Event is online or offline
        public bool IsOnline { get; set; } = false;

        //References to Who created the event
        public int CreatedByUserId { get; set; }
        [ValidateNever]
        [ForeignKey("CreatedByUserId")]
        public User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
