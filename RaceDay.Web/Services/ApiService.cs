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
            AddAuthCookie(); // <-- Make sure this is at the top!
            string imageUrl = "https://via.placeholder.com/600x300?text=RaceDay+Event";

            // 1. Upload Image via API
            if (image != null && image.Length > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(image.OpenReadStream()), "file", image.FileName);

                // We need to pass the auth cookie again just for this request to be safe
                var request = new HttpRequestMessage(HttpMethod.Post, "api/blob/upload");
                request.Content = content;

                var cookie = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookie))
                {
                    request.Headers.Add("Cookie", cookie);
                }

                var uploadRes = await _http.SendAsync(request);

                if (uploadRes.IsSuccessStatusCode)
                {
                    var json = await uploadRes.Content.ReadFromJsonAsync<JsonNode>();
                    imageUrl = json?["url"]?.ToString() ?? imageUrl;
                }
                else
                {
                    var uploadError = await uploadRes.Content.ReadAsStringAsync();
                    return $"Upload failed with Status: {uploadRes.StatusCode}. Details: {uploadError}";
                }
            }

            // ... rest of the Create Event code ...

            // 2. Create Event
            var eventPayload = new
            {
                EventName = data.GetType().GetProperty("EventName")?.GetValue(data)?.ToString(),
                Description = data.GetType().GetProperty("Description")?.GetValue(data)?.ToString(),
                EventDate = data.GetType().GetProperty("EventDate")?.GetValue(data),
                Location = data.GetType().GetProperty("Location")?.GetValue(data)?.ToString(),
                Distance = data.GetType().GetProperty("Distance")?.GetValue(data),
                EventType = data.GetType().GetProperty("EventType")?.GetValue(data)?.ToString(),
                ImageUrl = imageUrl
            };

            var response = await _http.PostAsJsonAsync("api/events", eventPayload);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                return $"Failed to create event. Status: {response.StatusCode}. Details: {errorDetails}";
            }

            return null;
        }
        public async Task<bool> DeleteEventAsync(int eventId)
        {
            AddAuthCookie();
            var response = await _http.DeleteAsync($"api/events/{eventId}");
            return response.IsSuccessStatusCode;
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
        public async Task<bool> AddResultAsync(int enrolmentId, int finishPosition, string finishTime)
        {
            AddAuthCookie();
            // The API expects a TimeSpan for FinishTime. "00:31:45" format.
            var payload = new
            {
                EnrolmentID = enrolmentId,
                FinishPosition = finishPosition,
                FinishTime = finishTime,
                ResultStatus = "Finished"
            };

            var response = await _http.PostAsJsonAsync("api/results", payload);
            return response.IsSuccessStatusCode;
        }
    }
}