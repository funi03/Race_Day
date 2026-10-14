using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class EventEnrolment
    {
        // Primary key
        public int EnrolmentID { get; set; }

        // Foreign keys
        public int ParticipantID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }

        // Properties
        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Registered";

        // Navigation properties
        public Participant? Participant { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
        public Result? Result { get; set; }
    }
}