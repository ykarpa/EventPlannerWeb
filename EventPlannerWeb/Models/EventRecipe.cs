using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPlannerWeb.Models
{
    public class EventRecipe
    {
        public int EventRecipeId { get; set; }

        [JsonIgnore]
        public Event? Event { get; set; }

        public int EventId { get; set; }

        [JsonIgnore]
        public Recipe? Recipe { get; set; }

        public int RecipeId { get; set; }
    }
}
