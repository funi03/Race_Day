using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _api;
        public ParticipantController(ApiService api) { _api = api; }

        private bool IsParticipant() => HttpContext.Session.GetString("Role") == "Participant";

        public async Task<IActionResult> Index()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var events = await _api.GetEventsAsync();
            return View(events);
        }

        public async Task<IActionResult> MyEnrolments()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var enrolments = await _api.GetMyEnrolmentsAsync();
            return View(enrolments);
        }

        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var results = await _api.GetMyResultsAsync();
            return View(results);
        }
    }
}
