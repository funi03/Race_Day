using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;
        public AccountController(ApiService api) { _api = api; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var result = await _api.LoginAsync(model.Email, model.Password);
            if (result.Success)
            {
                HttpContext.Session.SetString("Role", result.Role!);
                HttpContext.Session.SetString("Email", model.Email);

                if (result.Role == "Organiser") return RedirectToAction("Dashboard", "Organiser");
                return RedirectToAction("Index", "Participant");
            }
            ViewBag.Error = result.Error;
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("RaceDayAuth");
            return RedirectToAction("Login");
        }
    }
}