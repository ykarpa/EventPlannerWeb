using Microsoft.AspNetCore.Mvc;

namespace EventPlannerWeb.Controllers
{
    public class IngredientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
