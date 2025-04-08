using Fillow.Models;
using Fillow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fillow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;
		public AccountController(IConfiguration configuration,ILogger<HomeController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;

		}

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _userService.GetUserByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var user = new UserModel
                {
                    Username = model.Username,
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = hashedPassword,
                    UserType = model.UserType,
                    AcceptTerms = model.AcceptTerms
                };

                await _userService.CreateUserAsync(user);
                return Ok(new { message = "Create account successful!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                // Fetch user by email
                var user = await _userService.GetUserByEmailAsync(loginModel.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Generate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
                        // Add more claims as needed
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Issuer"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

				HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("UserType", user.UserType);

				return Ok(new { Token = tokenString });
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message.ToString());
            }
            return StatusCode(500, "Internal server error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}