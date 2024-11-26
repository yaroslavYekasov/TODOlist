using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOlist.Data;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthContext _context;

    public AuthController(AuthContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            return BadRequest("User already exists.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully!");
    }

    [HttpGet("login/{email}/{password}")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return Unauthorized("Invalid email or password.");

        return Ok("Login successful!");
    }
}
