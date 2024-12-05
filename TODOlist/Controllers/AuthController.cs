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

        // POST: api/Auth/register
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

            // Check if user exists and password matches
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                // Check for admin credentials (admin@gmail.com with password 'admin')
                if (loginDto.Email == "admin@gmail.com" && loginDto.Password == "admin")
                {
                    // Simulate admin login (no need to hash password for admin)
                    user = new User
                    {
                        Email = "admin@gmail.com",
                        Password = "admin", // Admin password
                        Role = "admin" // Set role as admin
                    };
                }
                else
                {
                    return Unauthorized("Invalid email or password.");
                }
            }

            // Set session with role
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);

            return Ok(new { message = "Login successful!", userId = user.Id, role = user.Role });
        }


        // GET: api/Auth/checksession
        [HttpGet("checksession")]
        public IActionResult CheckSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            return Ok(new { username = user.Email });
        }

        // PUT: api/Auth/updateUserInfo
        [HttpPut("updateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto updateDto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the provided current password is correct
            if (!BCrypt.Net.BCrypt.Verify(updateDto.Password, user.Password))
            {
                return BadRequest("Current password is incorrect.");
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                user.Email = updateDto.Email;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "User info updated successfully!" });
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out successfully.");
        }
    }
}
