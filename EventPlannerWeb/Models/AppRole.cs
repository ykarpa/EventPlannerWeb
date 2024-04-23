using Microsoft.AspNetCore.Identity;

namespace Library_kursova.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
