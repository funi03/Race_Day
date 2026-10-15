using Race_Day.Data;
using Race_Day.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    /// <summary>
    /// Manages RaceDay events.
    /// GET endpoints are public; POST/PUT/DELETE require the Organiser role
    /// and enforce that the Organiser owns the event being changed.
    /// </summary>
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EventsController(ApplicationDBContext context)
        {
            _context = context;
        }

        private async Task<Organiser?> CurrentOrganiserAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
        }

        /// <summary>Returns all RaceDay events. Public.</summary>
        /// <response code="200">List of events.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        /// <summary>Returns one event by ID. Public.</summary>
        /// <response code="200">Event found.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            return ev == null ? NotFound() : Ok(ev);
        }

        /// <summary>Creates a new event. Organiser only.</summary>
        /// <response code="201">Event created.</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="403">Not an Organiser.</response>
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] Event ev)
        {
            var organiser = await CurrentOrganiserAsync();
            if (organiser == null) return Forbid();

            ev.OrganiserID = organiser.OrganiserID;
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ev.EventID }, ev);
        }

        /// <summary>Updates own event. Organiser only, ownership enforced.</summary>
        /// <response code="200">Event updated.</response>
        /// <response code="403">Not the owner of this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Event updated)
        {
            var organiser = await CurrentOrganiserAsync();
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            if (organiser == null || ev.OrganiserID != organiser.OrganiserID) return Forbid();

            ev.EventName = updated.EventName;
            ev.Description = updated.Description;
            ev.EventDate = updated.EventDate;
            ev.Location = updated.Location;
            ev.Distance = updated.Distance;
            ev.EventType = updated.EventType;

            await _context.SaveChangesAsync();
            return Ok(ev);
        }

        /// <summary>Deletes own event. Organiser only, ownership enforced.</summary>
        /// <response code="204">Event deleted.</response>
        /// <response code="403">Not the owner of this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var organiser = await CurrentOrganiserAsync();
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            if (organiser == null || ev.OrganiserID != organiser.OrganiserID) return Forbid();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}