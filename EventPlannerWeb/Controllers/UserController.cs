using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly EventPlannerContext _context;

        public UserController(EventPlannerContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return user;
        }


        [HttpPost]
        public async Task<ActionResult> AddUser(User user)
        {
            //even.EventGuests
            if (ModelState.IsValid)
            {
                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok();
            }
            var message = GetModelValidationErrors();

            return BadRequest(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(e => e.UserId == id);
            if (user == default) return NotFound();

            _context.User.Remove(user);

            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<ActionResult> UpdateUser(User user)
        {
            if (user.UserId == default || !UserExists(user.UserId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

        public IActionResult Index()
        {
            return View();
        }

        private IEnumerable<string> GetModelValidationErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage);
        }
    }
}
