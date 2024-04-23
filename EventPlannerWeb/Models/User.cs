using Library_kursova.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPlannerWeb.Models
{
    public class User : IdentityUser<int>
    {
        public string Surname { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Event>? Events { get; set; }

        [JsonIgnore]
        public virtual ICollection<Guest>? Guests { get; set; }

        public Gender? Gender { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? UserImageName { get; set; }

        [JsonIgnore]
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}
