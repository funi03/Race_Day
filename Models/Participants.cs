namespace Race_Day.Models
{
    public class Participants
    {
        public int ParticipantID { get; set; }
        public int UserID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmergencyContact { get; set; } = string.Empty;

        public Users? User { get; set; }
        public List<EventEnrolments> Enrolments { get; set; } = new();
    }
}
