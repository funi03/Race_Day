using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class EventEnrolments
    {
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public DateTime EnrolmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Registered"; // Registered, Cancelled, Completed

        public Participants? Participant { get; set; }
        public Events? Event { get; set; }
        public Categories? Category { get; set; }
        public Results? Result { get; set; }
    }
}
