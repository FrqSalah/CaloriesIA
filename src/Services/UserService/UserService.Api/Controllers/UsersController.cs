using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using UserService.Api.Models;
using UserService.Api.Dtos;
using UserService.Api.Data;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UsersController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return Conflict("A user with this username or email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerDto.Username,
                Email = registerDto.Email,
                // WARNING: This is a very insecure way to handle passwords and is for development purposes only.
                // In production, use a proper password hashing algorithm like BCrypt, Argon2, ou PBKDF2
                PasswordHash = registerDto.Password + "_hashed"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                new { user.Id, user.Username, user.Email }
            );
        }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        // WARNING: This is a très insecure way to handle passwords and is for development purposes only.
        // In production, use a proper password hashing algorithm and secure comparison methods
        if (user == null || user.PasswordHash != loginDto.Password + "_hashed")
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(new { message = "Login successful" });
    }

    // Endpoint pour démarrer l'authentification Google (redirection vers Google)
    [HttpGet("google-login")]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Users", new { returnUrl }, Request.Scheme);
        var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }

    // Endpoint de callback pour Google OAuth
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Google");
        if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
        {
            return Unauthorized();
        }

        var email = authenticateResult.Principal.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var googleId = authenticateResult.Principal.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = authenticateResult.Principal.Identity?.Name ?? email ?? "googleuser";

        if (email == null || googleId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId || u.Email == email);
        if (user == null)
        {
            // Création d'un nouvel utilisateur Google
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                GoogleId = googleId,
                PasswordHash = string.Empty // Pas de mot de passe local
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Ici, vous pouvez générer un token JWT ou gérer la session selon vos besoins
        return Ok(new { message = "Google login successful", user.Id, user.Username, user.Email });
    }

    // This action is needed for CreatedAtAction to work in the Register method
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user.Id, user.Username, user.Email });
    }
    }
}
