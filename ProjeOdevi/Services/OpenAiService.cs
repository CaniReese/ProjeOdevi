using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ProjeOdevi.Services
{
    public class OpenAiService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public OpenAiService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task<string> GenerateExercisePlan(string prompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var model = _config["OpenAI:Model"] ?? "gpt-4o-mini";

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var response = await _http.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
