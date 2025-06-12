using ExpoBookApp.Models;

namespace ExpoBookApp.Models
{
    public class EventIndexViewModel
    {
        public List<Event> UpcomingEvents { get; set; }
        public List<Event> AllEvents { get; set; }
        public List<string> Themes { get; set; }
        public string ThemeFilter { get; set; }
    }
}
