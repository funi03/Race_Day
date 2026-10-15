using Race_Day.Data;
using Race_Day.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    /// <summary>
    /// Lets the logged-in user view and update their own profile.
    /// "Me" endpoints only — never accepts an ID from the client.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UsersController(ApplicationDBContext context)
        {
            _context = context;
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        /// <summary>Returns the logged-in user's own profile.</summary>
        /// <response code="200">Profile returned (never includes PasswordHash).</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMe()
        {
            var user = await _context.Users.FindAsync(CurrentUserId());
            if (user == null) return NotFound();

            return Ok(new { user.UserID, user.FullName, user.Email, user.Role });
        }

        /// <summary>Updates the logged-in user's own profile.</summary>
        /// <response code="200">Profile updated.</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDTO dto)
        {
            var user = await _context.Users.FindAsync(CurrentUserId());
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserID != user.UserID))
                    return BadRequest("Email already in use.");

                user.Email = dto.Email;
            }

            await _context.SaveChangesAsync();

            return Ok(new { user.UserID, user.FullName, user.Email, user.Role });
        }
    }
}
