using Microsoft.Extensions.Logging;

namespace Race_Day.Models
{
    public class Organisers
    {
        public int OrganiserID { get; set; }
        public int UserID { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;

        public Users? User { get; set; }
        public List<Events> Events { get; set; } = new();
    }
}
