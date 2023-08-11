using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JWT_WebAPI_Tutorial_Mac.Controllers
{
    public class AuthController : Controller
    {
#pragma warning disable IDE1006 // Naming Styles
        public static User user { get; set; } = new User();
#pragma warning restore IDE1006 // Naming Styles

        [HttpPost("register")]

        public ActionResult<User> Register(UserDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return Ok(user);
        }

        [HttpPost("login")]

        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            BadRequestObjectResult badResponse = BadRequest("User Not Found");
            OkObjectResult goodResponse = Ok("looking good");

            return (user.Username != request.Username) ? badResponse : goodResponse;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

        }



        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }


}

