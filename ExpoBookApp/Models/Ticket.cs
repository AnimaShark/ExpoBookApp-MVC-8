namespace ExpoBookApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public required string TicketCode { get; set; }
        public int? EventId { get; set; }
        public int? UserId { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }

        //Foreign Keys
        public Event? Event { get; set; }
        public User? User { get; set; }
    }

}
