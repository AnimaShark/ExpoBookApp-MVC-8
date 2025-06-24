using ExpoBookApp.Models;

namespace ExpoBookApp.Models
{
    public class EventIndexViewModel
    {
        public List<Event> CreatedEvents { get; set; } // For Organizer
        public List<Event> CreatedUpcomingEvents { get; set; } //For Organizer
        public List<Event> UpcomingEvents { get; set; }
        public List<Event> AllEvents { get; set; }
        public List<string> Themes { get; set; }
        public string ThemeFilter { get; set; }
    }
}
