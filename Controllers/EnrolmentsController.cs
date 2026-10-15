using Race_Day.Data;
using Race_Day.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    /// <summary>
    /// Manages event enrolments.
    /// Participants enrol and view their own enrolments.
    /// Organisers view enrolments for their own events.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EnrolmentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        /// <summary>Enrols the logged-in Participant into a category.</summary>
        /// <response code="201">Enrolment created.</response>
        /// <response code="403">Not a Participant.</response>
        /// <response code="404">Category not found.</response>
        [HttpPost("enrolments")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Enrol([FromBody] EnrolRequestDTO dto)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == CurrentUserId());
            if (participant == null) return Forbid();

            var category = await _context.Categories.FindAsync(dto.CategoryID);
            if (category == null) return NotFound("Category not found.");

            var enrolment = new EventEnrolment
            {
                ParticipantID = participant.ParticipantID,
                EventID = category.EventID,
                CategoryID = category.CategoryID,
                Status = "Registered"
            };

            _context.EventEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }

        /// <summary>Returns the logged-in Participant's own enrolments.</summary>
        /// <response code="200">List of own enrolments.</response>
        /// <response code="403">Not a Participant.</response>
        [HttpGet("enrolments/me")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MyEnrolments()
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == CurrentUserId());
            if (participant == null) return Forbid();

            var list = await _context.EventEnrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>Lists enrolments for one of the Organiser's own events.</summary>
        /// <response code="200">List of enrolments.</response>
        /// <response code="403">Not the owner of the event.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("events/{eventId}/enrolments")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EventEnrolments(int eventId)
        {
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == CurrentUserId());
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound();
            if (organiser == null || ev.OrganiserID != organiser.OrganiserID) return Forbid();

            var list = await _context.EventEnrolments
                .Include(e => e.Participant)
                .Where(e => e.EventID == eventId)
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>Cancels the logged-in Participant's own enrolment.</summary>
        /// <response code="200">Enrolment cancelled.</response>
        /// <response code="403">Not the owner of this enrolment.</response>
        /// <response code="404">Enrolment not found.</response>
        [HttpPut("enrolments/{id}/cancel")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == CurrentUserId());
            var enrolment = await _context.EventEnrolments.FindAsync(id);
            if (enrolment == null) return NotFound();
            if (participant == null || enrolment.ParticipantID != participant.ParticipantID) return Forbid();

            enrolment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }
    }
}