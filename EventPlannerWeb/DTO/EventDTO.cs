using EventPlannerWeb.Models;

namespace EventPlannerWeb.DTO
{
    public class EventDTO
    {
        public Event Event { get; set; }

        public List<int> Guests { get; set; }

        public List<int> Recipes { get; set; }
    }
}
