using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpoBookApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        public string EventName { get; set; }
        public string Theme { get; set; }
        public string Venue { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Ticket price cannot be negative."), DataType(DataType.Currency)]
        public decimal TicketPrice { get; set; }
        public int TicketBought { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Ticket quota cannot be negative.")]
        public int TicketQuota { get; set; }

        //Marking for public event
        public bool IsPublic { get; set; } = false;

        //Image for the event
        public string? ImagePath { get; set; }

        //References to Who created the event
        public int CreatedByUserId { get; set; }

        [ValidateNever]
        [ForeignKey("CreatedByUserId")]
        public User CreatedBy { get; set; }

    }
}
