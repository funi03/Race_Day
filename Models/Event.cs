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
        public string EventType { get; set; } = string.Empty; // Run, Walk, Cycle

        public Organiser? Organiser { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<EventEnrolment> Enrolments { get; set; } = new();
    }
}
