﻿using ExpoBookApp.Models;

namespace ExpoBookApp.Models
{
    public class EventIndexViewModel
    {
        public List<Event> CreatedEvents { get; set; } // For Organizer
        public List<Event> CreatedUpcomingEvents { get; set; } //For Organizer
        public List<Event> UpcomingEvents { get; set; }
        public List<Event> DeletedEvents { get; set; }
        public List<Event> AllEvents { get; set; }
        public List<string> EventType { get; set; }
        public List<string> CreatedBy { get; set; } // Organizer email addresses 
        public string TypeFilter { get; set; }
    }
}
