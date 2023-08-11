using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens; // added packages from nuget
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JWT_WebAPI_Tutorial_Mac.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
#pragma warning disable IDE1006 // Naming Styles
        public static User user { get; set; } = new User();
#pragma warning restore IDE1006 // Naming Styles

        // add constructor and internal prop to reference the config
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


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

        public ActionResult<string> Login(UserDTO request)
        {
            BadRequestObjectResult noUserFound = BadRequest("User Not Found");
            BadRequestObjectResult badPassword = BadRequest("Invalid Password");
            OkObjectResult goodResponse = Ok("looking good");

            // add passwordHash verification
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return badPassword;
            }

            if (user.Username != request.Username)
            {
                return noUserFound;
            }


            // create the token
            string token = CreateToken(user);

            return token;

        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {

                new Claim(ClaimTypes.Name, user.Username)

            };

            // create a new security key
            // go to appsettings.json and add config setting
            // then add config setting to create the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            // then add signing credentials
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // define the properties of the payload of the JSON web token
            // add new web token
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

            // throw new NotImplementedException();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        }

        private static bool VerifyPasswordHash(string password, byte[]? passwordHash, byte[]? passwordSalt)
        {

            using (var hmac = new HMACSHA512(key: passwordSalt))
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

