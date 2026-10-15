using Race_Day.Data;
using Race_Day.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    /// <summary>
    /// Manages race results.
    /// Public GET for an event's results; Organiser-only POST/PUT (ownership enforced);
    /// Participant-only GET for their own results.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ResultsController(ApplicationDBContext context)
        {
            _context = context;
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        /// <summary>Returns all results for one event. Public.</summary>
        /// <response code="200">List of results.</response>
        [HttpGet("events/{eventId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EventResults(int eventId)
        {
            var results = await _context.Results
                .Include(r => r.Enrolment)
                .Where(r => r.Enrolment!.EventID == eventId)
                .ToListAsync();

            return Ok(results);
        }

        /// <summary>Records a result for an enrolment. Organiser only, ownership enforced.</summary>
        /// <response code="201">Result recorded.</response>
        /// <response code="403">Not the owner of the event.</response>
        /// <response code="404">Enrolment not found.</response>
        [HttpPost("results")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] Result result)
        {
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == CurrentUserId());

            var enrolment = await _context.EventEnrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == result.EnrolmentID);
            if (enrolment == null) return NotFound("Enrolment not found.");
            if (organiser == null || enrolment.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        /// <summary>Updates a result. Organiser only, ownership enforced.</summary>
        /// <response code="200">Result updated.</response>
        /// <response code="403">Not the owner.</response>
        /// <response code="404">Result not found.</response>
        [HttpPut("results/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Result updated)
        {
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == CurrentUserId());

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);
            if (result == null) return NotFound();
            if (organiser == null || result.Enrolment!.Event!.OrganiserID != organiser.OrganiserID)
                return Forbid();

            result.FinishPosition = updated.FinishPosition;
            result.FinishTime = updated.FinishTime;
            result.ResultStatus = updated.ResultStatus;

            await _context.SaveChangesAsync();
            return Ok(result);
        }

        /// <summary>Returns the logged-in Participant's own results across all events.</summary>
        /// <response code="200">List of own results.</response>
        /// <response code="403">Not a Participant.</response>
        [HttpGet("participants/me/results")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MyResults()
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == CurrentUserId());
            if (participant == null) return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .Where(r => r.Enrolment!.ParticipantID == participant.ParticipantID)
                .ToListAsync();

            return Ok(results);
        }
    }
}