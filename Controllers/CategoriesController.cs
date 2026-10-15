using Race_Day.Data;
using Race_Day.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    /// <summary>
    /// Manages categories within events.
    /// GET is public; POST/PUT/DELETE require Organiser role and enforce
    /// ownership one level removed (Category → Event → OrganiserID).
    /// </summary>
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        private async Task<Organiser?> CurrentOrganiserAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
        }

        /// <summary>Lists all categories for an event. Public.</summary>
        /// <response code="200">List of categories.</response>
        [HttpGet("events/{eventId}/categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForEvent(int eventId)
        {
            var cats = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();
            return Ok(cats);
        }

        /// <summary>Creates a category on own event. Organiser only.</summary>
        /// <response code="201">Category created.</response>
        /// <response code="403">Not the owner of the event.</response>
        /// <response code="404">Event not found.</response>
        [HttpPost("events/{eventId}/categories")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int eventId, [FromBody] Category category)
        {
            var organiser = await CurrentOrganiserAsync();
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound();
            if (organiser == null || ev.OrganiserID != organiser.OrganiserID) return Forbid();

            category.EventID = eventId;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        /// <summary>Updates own category. Organiser only.</summary>
        /// <response code="200">Category updated.</response>
        /// <response code="403">Not the owner.</response>
        /// <response code="404">Category not found.</response>
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Category updated)
        {
            var organiser = await CurrentOrganiserAsync();
            var cat = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);
            if (cat == null) return NotFound();
            if (organiser == null || cat.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            cat.CategoryName = updated.CategoryName;
            cat.DistanceKm = updated.DistanceKm;
            cat.MinimumAge = updated.MinimumAge;
            cat.MaximumAge = updated.MaximumAge;
            cat.MaximumParticipants = updated.MaximumParticipants;

            await _context.SaveChangesAsync();
            return Ok(cat);
        }

        /// <summary>Deletes own category. Organiser only.</summary>
        /// <response code="204">Category deleted.</response>
        /// <response code="403">Not the owner.</response>
        /// <response code="404">Category not found.</response>
        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var organiser = await CurrentOrganiserAsync();
            var cat = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);
            if (cat == null) return NotFound();
            if (organiser == null || cat.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}