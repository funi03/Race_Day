using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class EventEnrolment
    {
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Registered";

        // Navigation properties — note the CAPITAL R and the TYPE Result
        public Participant? Participant { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
        public Result? Result { get; set; }        
    }
}