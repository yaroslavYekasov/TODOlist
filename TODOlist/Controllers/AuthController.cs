// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOlist.Data;
using Microsoft.AspNetCore.Http;

namespace TODOlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _context;

        public AuthController(AuthContext context)
        {
            _context = context;
        }

        // Controllers/AuthController.cs
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("User already exists.");

            var user = new User
            {
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Set session
            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new { message = "User registered successfully!", userId = user.Id });
        }


        // Controllers/AuthController.cs
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return Unauthorized("Invalid email or password.");

            // Set session
            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new { message = "Login successful!", userId = user.Id });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out successfully.");
        }
    }
}
