using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Race_Day.Data;
using Race_Day.Models;
using System.Security.Claims;

namespace Race_Day.Controllers
{
    [ApiController]
    [Route("api")]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ResultsController(ApplicationDBContext context)
        {
            _context = context;
        }

        private async Task<Organiser?> CurrentOrganiserAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return null;
            var userId = int.Parse(userIdString);
            return await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
        }

        /// <summary>Returns all results for a specific event. Public.</summary>
        [HttpGet("events/{eventId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EventResults(int eventId)
        {
            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                        .ThenInclude(p => p.User)
                .Where(r => r.Enrolment!.EventID == eventId)
                .ToListAsync();

            return Ok(results);
        }

        /// <summary>Records a result for an enrolment. Organiser only.</summary>
        [HttpPost("results")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] Result result)
        {
            var organiser = await CurrentOrganiserAsync();
            if (organiser == null) return Forbid();

            // Find the enrolment to ensure it belongs to this organiser's event
            var enrolment = await _context.EventEnrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == result.EnrolmentID);

            if (enrolment == null) return NotFound("Enrolment not found.");
            if (enrolment.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            // Check if a result already exists for this enrolment (1-to-1 relationship)
            var existingResult = await _context.Results.FirstOrDefaultAsync(r => r.EnrolmentID == result.EnrolmentID);
            if (existingResult != null)
            {
                return BadRequest("A result already exists for this enrolment.");
            }

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        /// <summary>Updates a result. Organiser only.</summary>
        [HttpPut("results/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Result updated)
        {
            var organiser = await CurrentOrganiserAsync();
            if (organiser == null) return Forbid();

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null) return NotFound();
            if (result.Enrolment!.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            result.FinishPosition = updated.FinishPosition;
            result.FinishTime = updated.FinishTime;
            result.ResultStatus = updated.ResultStatus;

            await _context.SaveChangesAsync();
            return Ok(result);
        }

        /// <summary>Returns the logged-in Participant's own results. Participant only.</summary>
        [HttpGet("participants/me/results")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MyResults()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Forbid();
            var userId = int.Parse(userIdString);

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                .Where(r => r.Enrolment!.ParticipantID == participant.ParticipantID)
                .ToListAsync();

            return Ok(results);
        }
    }
}
