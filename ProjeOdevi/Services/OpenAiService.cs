using OpenAI.Chat;

namespace ProjeOdevi.Services
{
    public class OpenAiService
    {
        private readonly ChatClient _chat;

        public OpenAiService(IConfiguration config)
        {
            var apiKey = config["OpenAI:ApiKey"];
            var model = config["OpenAI:Model"] ?? "gpt-4o-mini"; // istersen değiştir

            _chat = new ChatClient(model: model, apiKey: apiKey);
        }

        public async Task<string> CreateWorkoutPlanAsync(int age, int heightCm, int weightKg, string goal)
        {
            string prompt = $"""
            Kullanıcı bilgileri:
            - Yaş: {age}
            - Boy: {heightCm} cm
            - Kilo: {weightKg} kg
            - Hedef: {goal}

            7 günlük egzersiz planı üret.
            Her gün için:
            - Isınma
            - Ana antrenman (set/tekrar)
            - Soğuma
            En sona kısa beslenme önerisi ekle.
            Türkçe yaz, madde madde yaz.
            """;

            // İstersen direkt string de verebilirsin:
            ChatCompletion completion = await _chat.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }
    }
}
