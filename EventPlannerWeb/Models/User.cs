using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPlannerWeb.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public virtual ICollection<Event>? Events { get; set; }

        [JsonIgnore]
        public virtual ICollection<Guest>? Guests { get; set; }

        public Gender? Gender { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? UserImageName { get; set; }

    }
}
