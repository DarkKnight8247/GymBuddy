using Microsoft.AspNetCore.Mvc;

namespace Gymbuddy.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly IHttpClientFactory _http;
        private const string BASE    = "https://api.api-ninjas.com/v1/exercises";
        private const string API_KEY = "gTNryaGkvmOx3VbWOOKsMRksD8JfTqhIxxv94cSR";

        public ExerciseController(IHttpClientFactory http) => _http = http;

        // GET /   and   GET /Exercise
        public IActionResult Index() => View();

public async Task<IActionResult> Test()
{
    try
    {
        var client = _http.CreateClient("exercisedb");
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.api-ninjas.com/v1/exercises?muscle=lower_back");
        request.Headers.Add("X-Api-Key", API_KEY);
        
        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        
        return Content($"Status: {(int)response.StatusCode}\nBody: {body}", "text/plain");
    }
    catch (Exception ex)
    {
        return Content($"Exception: {ex.GetType().Name}\nMessage: {ex.Message}\nInner: {ex.InnerException?.Message}", "text/plain");
    }
}

        // GET /Exercise/ByMuscle?muscle=biceps&offset=0
        // GET /Exercise/ByMuscle?type=cardio&offset=0
        public async Task<IActionResult> ByMuscle(string? muscle, string? type, string? difficulty, int offset = 0)
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
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return StatusCode(502, new { status = (int)response.StatusCode, error = body, url = url });
                return Content(body, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { error = ex.Message });
            }
        }
    }
}
