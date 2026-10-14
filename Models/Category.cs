using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class Category
    {
        public int CategoryID { get; set; }        // ← PRIMARY KEY — must exist
        public int EventID { get; set; }           // ← FK to Events
        public string CategoryName { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public int MaximumParticipants { get; set; }

        // Navigation
        public Event? Event { get; set; }
        public List<EventEnrolment> Enrolments { get; set; } = new();
    }
}
    
