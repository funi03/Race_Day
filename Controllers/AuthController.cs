using Race_Day.Data;
using Race_Day.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    /// <summary>
    /// Handles user registration, login, and logout.
    /// Passwords are hashed with BCrypt; login issues a cookie session.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AuthController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>Registers a new user (Organiser or Participant).</summary>
        /// <response code="200">User registered successfully. Returns ID, name, email, role.</response>
        /// <response code="400">Email already exists, or role is invalid.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (dto.Role != "Organiser" && dto.Role != "Participant")
                return BadRequest("Role must be 'Organiser' or 'Participant'.");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Auto-create the matching profile
            if (dto.Role == "Organiser")
            {
                _context.Organisers.Add(new Organiser
                {
                    UserID = user.UserID,
                    OrganisationName = dto.FullName + " Organisation",
                    ContactNumber = "0000000000"
                });
            }
            else
            {
                _context.Participants.Add(new Participant
                {
                    UserID = user.UserID,
                    DateOfBirth = new DateTime(2000, 1, 1),
                    EmergencyContact = "N/A"
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { user.UserID, user.FullName, user.Email, user.Role });
        }

        /// <summary>Authenticates a user and creates a session cookie.</summary>
        /// <response code="200">Login successful. Cookie set.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok(new { user.UserID, user.FullName, user.Email, user.Role });
        }

        /// <summary>Logs out the current user and clears the session cookie.</summary>
        /// <response code="200">Logged out.</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out.");
        }
    }
}