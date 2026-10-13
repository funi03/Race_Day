using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class Categories
    {
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public int MaximumParticipants { get; set; }

        public Events? Event { get; set; }
        public List<EventEnrolments> Enrolments { get; set; } = new();
    }
}
