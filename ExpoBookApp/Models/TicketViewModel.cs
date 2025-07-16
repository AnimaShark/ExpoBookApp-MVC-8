using System;

namespace ExpoBookApp.Models
{
    public class TicketViewModel
    {
        public string TicketCode { get; set; }
        public string EventName { get; set; }
        public string Venue { get; set; }
        public DateTime? StartDate { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsEventMissing { get; set; }

        //Pull event details from the Event model
        public Event Event { get; set; }
    }

}