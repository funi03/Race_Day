using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class EventEnrolment
    {
        public int EnrolmentID { get; set; }        // ← PRIMARY KEY — must exist
        public int ParticipantID { get; set; }      // ← FK
        public int EventID { get; set; }            // ← FK
        public int CategoryID { get; set; }         // ← FK
        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Registered";

        // Navigation
        public Participant? Participant { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
        public result? Result { get; set; }
        public object result { get; internal set; }
    }
}
