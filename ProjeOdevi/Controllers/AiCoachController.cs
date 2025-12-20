using Microsoft.AspNetCore.Mvc;
using ProjeOdevi.Services;

namespace ProjeOdevi.Controllers
{
    public class AiCoachController : Controller
    {
        private readonly OpenAiService _openAi;

        public AiCoachController(OpenAiService openAi)
        {
            _openAi = openAi;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int age, int heightCm, int weightKg, string goal)
        {
            if (age <= 0 || heightCm <= 0 || weightKg <= 0 || string.IsNullOrWhiteSpace(goal))
            {
                ViewBag.Error = "Lütfen yaş / boy / kilo ve hedef alanlarını doldurun.";
                return View();
            }

            var plan = await _openAi.CreateWorkoutPlanAsync(age, heightCm, weightKg, goal);
            ViewBag.Plan = plan;

            return View();
        }
    }
}
