using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeOdevi.Models;
using ProjeOdevi.Services;

[Authorize]
public class AiCoachController : Controller
{
    private readonly OpenAiService _ai;

    public AiCoachController(OpenAiService ai)
    {
        _ai = ai;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(AiPlanRequest model)
    {
        var prompt = $"""
        Yaş: {model.Age}
        Boy: {model.Height} cm
        Kilo: {model.Weight} kg
        Hedef: {model.Goal}

        Bu bilgilerle haftalık bir egzersiz planı hazırla.
        """;

        ViewBag.Result = await _ai.GenerateExercisePlan(prompt);
        return View();
    }
}
