using EventPlannerWeb.Models;

namespace EventPlannerWeb.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
