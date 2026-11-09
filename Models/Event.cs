using System.Text.Json.Serialization;

namespace Race_Day.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public Organiser? Organiser { get; set; }

        [JsonIgnore] 
        public List<Category> Categories { get; set; } = new();

        [JsonIgnore] 
        public List<EventEnrolment> Enrolments { get; set; } = new();
    }
}