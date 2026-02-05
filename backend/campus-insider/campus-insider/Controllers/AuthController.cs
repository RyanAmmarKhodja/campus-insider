using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using campus_insider.DTOs;
using campus_insider.Services;
using System.Threading.Tasks;

namespace campus_insider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _userService.GetByLogin(login);

            // Replace this with your actual user validation logic (e.g., check DB)
            // 2. Simple string comparison (WARNING: Temporary only!)
            if (user == null || user.Password != login.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

            // 3. Create the "Claims" (The user's identity data)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // This is the 'userId'
                new Claim(ClaimTypes.Email, user.Email),                  // This is the 'email'
                new Claim("FavoriteColor", "Blue")                        // You can even make custom claims!
            };

            // 4. Create the Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }
    }
}
