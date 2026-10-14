using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class Organiser
    {
        public int OrganiserID { get; set; }
        public int UserID { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;

        public User? User { get; set; }
        public List<Event> Events { get; set; } = new();
    }
}
