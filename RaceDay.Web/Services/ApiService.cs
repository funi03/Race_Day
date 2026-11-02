using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        // Helper to pass the browser's auth cookie to the API
        private void AddAuthCookie()
        {
            var cookie = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
            {
                _http.DefaultRequestHeaders.Remove("Cookie");
                _http.DefaultRequestHeaders.Add("Cookie", cookie);
            }
        }

        // --- AUTH ---
        public async Task<(bool Success, string? Role, string? Error)> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var cookieHeader = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
                if (cookieHeader != null)
                    _httpContextAccessor.HttpContext?.Response.Headers.Append("Set-Cookie", cookieHeader.Split(';')[0]);

                var json = await response.Content.ReadFromJsonAsync<JsonNode>();
                return (true, json?["role"]?.ToString(), null);
            }
            return (false, null, "Invalid credentials.");
        }

        public async Task RegisterAsync(string fullName, string email, string password, string role)
        {
            await _http.PostAsJsonAsync("api/auth/register", new { FullName = fullName, Email = email, Password = password, Role = role });
        }

        // --- EVENTS ---
        public async Task<JsonArray?> GetEventsAsync()
        {
            var response = await _http.GetAsync("api/events");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<JsonArray>() : null;
        }

        public async Task<JsonArray?> GetMyEventsAsync()
        {
            AddAuthCookie();
            var response = await _http.GetAsync("api/events");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<JsonArray>() : null;
        }

        public async Task<JsonArray?> GetCategoriesForEventAsync(int eventId)
        {
            var response = await _http.GetAsync($"api/events/{eventId}/categories");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<JsonArray>() : null;
        }

        public async Task<string?> CreateEventAsync(object data, IFormFile? image)
        {
            AddAuthCookie();
            string imageUrl = "https://via.placeholder.com/600x300?text=RaceDay+Event";

            // 1. Upload Image via API
            if (image != null && image.Length > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(image.OpenReadStream()), "file", image.FileName);
                var uploadRes = await _http.PostAsync("api/blob/upload", content);
                if (uploadRes.IsSuccessStatusCode)
                {
                    var json = await uploadRes.Content.ReadFromJsonAsync<JsonNode>();
                    imageUrl = json?["url"]?.ToString() ?? imageUrl;
                }
            }

            // 2. Create Event
            var eventPayload = new
            {
                EventName = data.GetType().GetProperty("EventName")?.GetValue(data),
                Description = data.GetType().GetProperty("Description")?.GetValue(data),
                EventDate = data.GetType().GetProperty("EventDate")?.GetValue(data),
                Location = data.GetType().GetProperty("Location")?.GetValue(data),
                Distance = data.GetType().GetProperty("Distance")?.GetValue(data),
                EventType = data.GetType().GetProperty("EventType")?.GetValue(data),
                ImageUrl = imageUrl
            };

            var response = await _http.PostAsJsonAsync("api/events", eventPayload);
            return response.IsSuccessStatusCode ? null : "Failed to create event.";
        }

        // --- ENROLMENTS & RESULTS ---
        public async Task<JsonArray?> GetMyEnrolmentsAsync()
        {
            AddAuthCookie();
            var response = await _http.GetAsync("api/enrolments/me");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<JsonArray>() : null;
        }

        public async Task<bool> EnrolAsync(int categoryId)
        {
            AddAuthCookie();
            var response = await _http.PostAsJsonAsync("api/enrolments", new { CategoryID = categoryId });
            return response.IsSuccessStatusCode;
        }

        public async Task<JsonArray?> GetMyResultsAsync()
        {
            AddAuthCookie();
            var response = await _http.GetAsync("api/participants/me/results");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<JsonArray>() : null;
        }

        public async Task<JsonArray?> GetEventEnrolmentsAsync(int eventId)
        {
            AddAuthCookie();
            var response = await _http.GetAsync($"api/events/{eventId}/enrolments");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<JsonArray>() : null;
        }
    }
}