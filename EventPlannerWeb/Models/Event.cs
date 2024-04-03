namespace EventPlannerWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class Event
    {
        public int EventId { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public ICollection<EventGuest>? EventGuests { get; set; }

        [JsonIgnore]
        public ICollection<EventRecipe>? EventRecipes { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public double Price { get; set; }
    }
}
