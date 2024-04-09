using EventPlannerWeb.Models;

namespace EventPlannerWeb.DTO
{
    public class EventDTO
    {
        public Event Event { get; set; }

        public List<Guest> Guests { get; set; }

        public List<Recipe> Recipes { get; set; }
    }
}
