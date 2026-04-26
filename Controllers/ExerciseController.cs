using Microsoft.AspNetCore.Mvc;

namespace Gymbuddy.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly IHttpClientFactory _http;

        // ── ExerciseDB via RapidAPI (free tier, 10 req/day on basic) ──
        private const string BASE    = "https://exercisedb.p.rapidapi.com/exercises";
        private const string API_KEY  = "01b80e6c84mshfb202b5cc9eef7bp173266jsnc1e3220b99bf";
        private const string API_HOST = "exercisedb.p.rapidapi.com";

        public ExerciseController(IHttpClientFactory http) => _http = http;

        public IActionResult Index() => View();

        // GET /Exercise/ByMuscle?muscle=biceps&offset=0&limit=50
        [HttpGet]
        public async Task<IActionResult> ByMuscle(
            string? muscle, string? bodyPart, int offset = 0, int limit = 50)
        {
            try
            {
                var client = _http.CreateClient("exercisedb");

                // ExerciseDB uses bodyPart, not muscle — map common values
                var target = bodyPart ?? MuscleToBodyPart(muscle ?? "back");
                var url = $"{BASE}/bodyPart/{Uri.EscapeDataString(target)}?offset={offset}&limit={limit}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", API_KEY);
                request.Headers.Add("X-RapidAPI-Host", API_HOST);

                var response = await client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode,
                        new { error = $"API {response.StatusCode}", body = json });

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Map API Ninjas muscle names → ExerciseDB bodyPart names
        private static string MuscleToBodyPart(string muscle) => muscle.ToLower() switch
        {
            "back"        or "lower_back" or "lats" or "traps" => "back",
            "chest"                                             => "chest",
            "shoulders"                                         => "shoulders",
            "biceps"      or "forearms"                         => "upper arms",
            "triceps"                                           => "upper arms",
            "quadriceps"  or "hamstrings" or "glutes"           => "upper legs",
            "calves"                                            => "lower legs",
            "abdominals"                                        => "waist",
            "neck"                                              => "neck",
            "cardio"                                            => "cardio",
            _                                                   => muscle
        };
    }
}
