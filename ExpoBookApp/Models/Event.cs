using System.Collections;
using System.ComponentModel.DataAnnotations;

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
        [Range(0, 999999), DataType(DataType.Currency)]
        public decimal TicketPrice { get; set; }
        public int TicketBought { get; set; }
        public int TicketQuota { get; set; }

    }
}
