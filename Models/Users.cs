namespace Race_Day.Models
{
    public class Users
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        // Navigation
        public Organisers? Organiser { get; set; }
        public Participants? Participant { get; set; }
    }
}
