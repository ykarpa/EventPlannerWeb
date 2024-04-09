using System.Text.Json.Serialization;

namespace EventPlannerWeb.Models
{
    public class EventGuest
    {
        public int EventGuestId { get; set; }

        [JsonIgnore]
        public Event? Event { get; set; }

        public int EventId { get; set; }

        [JsonIgnore]
        public Guest? Guest { get; set; }

        public int GuestId { get; set; }
    }
}