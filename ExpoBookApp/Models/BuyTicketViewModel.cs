namespace ExpoBookApp.Models
{
    public class BuyTicketViewModel
    {
        public Event? Event { get; set; }
        public int AlreadyBought { get; set; }
        public int MaxRemaining { get; set; }
    }
}
