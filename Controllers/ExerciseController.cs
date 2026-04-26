using Microsoft.AspNetCore.Mvc;

namespace Gymbuddy.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly IHttpClientFactory _http;
        private const string BASE    = "https://api.api-ninjas.com/v1/exercises";
        private const string API_KEY = "Gj5U7YoCoZhljCOIO0PoYMZmaA5sKIWTJxAdSt3Y";

        public ExerciseController(IHttpClientFactory http) => _http = http;

        // GET /  and  GET /Exercise
        public IActionResult Index() => View();

        // GET /Exercise/ByMuscle?muscle=biceps&offset=0
        [HttpGet]
        public async Task<IActionResult> ByMuscle(
            string? muscle, string? type, string? difficulty, int offset = 0)
        {
            try
            {
                var client = _http.CreateClient("exercisedb");

                var query = new List<string>();
                if (!string.IsNullOrEmpty(muscle))     query.Add($"muscle={Uri.EscapeDataString(muscle)}");
                if (!string.IsNullOrEmpty(type))       query.Add($"type={Uri.EscapeDataString(type)}");
                if (!string.IsNullOrEmpty(difficulty)) query.Add($"difficulty={Uri.EscapeDataString(difficulty)}");
                query.Add($"offset={offset}");

                var url = $"{BASE}?{string.Join("&", query)}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Api-Key", API_KEY);

                var response = await client.SendAsync(request);

                // Return API status + body even on failure so frontend can debug
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode,
                        new { error = $"API returned {response.StatusCode}", body = json });

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
