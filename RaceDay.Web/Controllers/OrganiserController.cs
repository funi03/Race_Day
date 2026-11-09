using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _api;
        public OrganiserController(ApiService api) { _api = api; }

        private bool IsOrganiser() => HttpContext.Session.GetString("Role") == "Organiser";

        public async Task<IActionResult> Dashboard()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            var events = await _api.GetMyEventsAsync();
            return View(events);
        }

        [HttpGet]
        public IActionResult CreateEvent() => IsOrganiser() ? View() : RedirectToAction("Login", "Account");

        [HttpPost]
        public async Task<IActionResult> CreateEvent(string eventName, string description, DateTime eventDate, string location, decimal distance, string eventType, IFormFile image)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var payload = new { EventName = eventName, Description = description, EventDate = eventDate, Location = location, Distance = distance, EventType = eventType };
            var error = await _api.CreateEventAsync(payload, image);

            if (error == null) return RedirectToAction("Dashboard");

            ViewBag.Error = error;
            return View();
        }

        public async Task<IActionResult> EventEnrolments(int id)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            var enrolments = await _api.GetEventEnrolmentsAsync(id);
            ViewBag.EventId = id;
            return View(enrolments);
        }

     
        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var success = await _api.DeleteEventAsync(id);

            if (success)
                TempData["Msg"] = "Event deleted successfully.";
            else
                TempData["Error"] = "Failed to delete event.";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
    public async Task<IActionResult> AddResult(int enrolmentId, int finishPosition, string finishTime, int eventId)
   {
    if (!IsOrganiser()) return RedirectToAction("Login", "Account");
    
    var success = await _api.AddResultAsync(enrolmentId, finishPosition, finishTime);
    
    if (success)
        TempData["Msg"] = "Result saved successfully!";
    else
        TempData["Error"] = "Failed to save result.";

    return RedirectToAction("EventEnrolments", new { id = eventId });
}

    }
}