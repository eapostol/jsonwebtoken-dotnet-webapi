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
            BadRequestObjectResult noUserFound = BadRequest("User Not Found");
            BadRequestObjectResult badPassword = BadRequest("Invalid Password");
            OkObjectResult goodResponse = Ok("looking good");

            // add passwordHash verification
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return badPassword;
            }

            if (user.Username != request.Username) {
                return noUserFound;
            }


            // create the token
            string token = CreateToken(user);

            return goodResponse;

        }

        private string CreateToken(User user)
        {
            return string.Empty;

            // throw new NotImplementedException();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        }

        private bool VerifyPasswordHash(string password, byte[]? passwordHash, byte[]? passwordSalt)
        {

            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash); // returns true if the sequence is equal
            }
        }



        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }


}

