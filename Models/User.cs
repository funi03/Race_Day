namespace Race_Day.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        // Navigation
        public Organiser? Organiser { get; set; }
        public Participant? Participant { get; set; }
    }
}
