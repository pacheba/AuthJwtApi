using AuthJwtApi.Models;
using AuthJwtApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthJwtApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<UserResponse> Register(RegisterRequest req)
        {
            try
            {
                var user = new User { Username = req.Username, Email = req.Email };
                var created = _userService.Create(user, req.Password);
                var resp = new UserResponse(created.Id, created.Username ?? "", created.Email ?? "");
                return CreatedAtAction(nameof(Register), resp);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> Login(LoginRequest req)
        {
            var user = _userService.GetByUsername(req.Username);
            if (user == null || !_userService.VerifyPassword(user, req.Password))
                return Unauthorized(new { message = "Invalid username or password" });

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings["Key"] ?? throw new Exception("JWT Key not found");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiresMinutes = int.Parse(jwtSettings["DurationInMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new AuthResponse(tokenString, expiresMinutes));
        }
    }
}
