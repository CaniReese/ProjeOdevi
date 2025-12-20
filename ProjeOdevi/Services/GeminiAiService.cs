using System.Text;
using System.Text.Json;

namespace ProjeOdevi.Services
{
    public class GeminiAiService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public GeminiAiService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task<string> GenerateExercisePlan(string prompt)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return "Gemini API key bulunamadı. User Secrets içine eklediğinizden emin olun.";

            var model = _config["Gemini:Model"] ?? "gemini-2-flash";

            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var response = await _http.PostAsync(
                url,
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Gemini API hata: {(int)response.StatusCode} {response.ReasonPhrase}\n{json}";
            }

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "(Boş yanıt döndü)";
        }
    }
}
