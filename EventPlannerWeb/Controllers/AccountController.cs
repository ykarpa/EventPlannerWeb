using System.Security.Cryptography;
using System.Text;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Interfaces;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;



namespace Library_kursova.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;

        public AccountController(UserManager<User> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpPost("register")] 
        public async Task<ActionResult<UserDTO>> Register(UserDTO registerDTO)
        {
            if (await UserExists(registerDTO.Email)) return BadRequest("User with that email address already exists");

            var user = new User
            {
                Surname = registerDTO.Surname,
                Name = registerDTO.Name,
                Gender = registerDTO.Gender,
                PhoneNumber = registerDTO.PhoneNumber,
                Email = registerDTO.Email.ToLower(),
                CreatedDate = DateTime.UtcNow,
                UserName = registerDTO.Surname
               
            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) return BadRequest("password");

            return new UserDTO
            {
                Surname = user.Surname,
                Name = user.Name,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email.ToLower(),
                Token = tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
                return Unauthorized("Invalid email");

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
                return Unauthorized("Invalid password");

            return new UserDTO
            {
                Surname = user.Surname,
                Name = user.Name,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email.ToLower(),
                Token = tokenService.CreateToken(user)
            };
        }


        private async Task<bool> UserExists(string email)
        {
            return await userManager.Users.AnyAsync(u => u.Email == email.ToLower());
        }






    }
}
