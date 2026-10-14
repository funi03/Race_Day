namespace Race_Day.Models
{
    public class Participant
    {
        public int ParticipantID { get; set; }
        public int UserID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmergencyContact { get; set; } = string.Empty;

        public User? User { get; set; }
        public List<EventEnrolment> Enrolments { get; set; } = new();
    }
}
