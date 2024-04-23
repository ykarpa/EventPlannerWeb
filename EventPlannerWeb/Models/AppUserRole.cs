using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace Library_kursova.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public User User { get; set; }

        public AppRole Role { get; set; }
    }
}
