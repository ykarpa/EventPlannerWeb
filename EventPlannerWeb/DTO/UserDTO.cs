using EventPlannerWeb.Models;
using System.Text.Json.Serialization;

namespace EventPlannerWeb.DTO
{
    public class UserDTO
    {
        public string Surname { get; set; }

        public string Name { get; set; }

        public Gender? Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string? UserImage { get; set; }

        public string Password { get; set; }

        public string? Token { get; set; }
    }
}
