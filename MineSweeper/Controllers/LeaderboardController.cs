using Microsoft.AspNetCore.Mvc;
using MineSweeper.Models.ViewModels;
using MineSweeper.Services;

namespace MineSweeper.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly IGameResultService _resultService;

        public LeaderboardController(IGameResultService resultService)
        {
            _resultService = resultService;
        }

        public async Task<IActionResult> Index()
        {
            var results = await _resultService.GetResultsAsync();

            var viewModel = new LeaderboardViewModel
            {
                AllResults = results,
                BestResultsByDifficulty = GroupResultsByDifficulty(results)
            };

            return View(viewModel);
        }

        private Dictionary<string, List<Models.GameResult>> GroupResultsByDifficulty(List<Models.GameResult> results)
        {
            var grouped = new Dictionary<string, List<Models.GameResult>>();

            // Групуємо за складністю (розмір поля + кількість мін)
            var difficultyGroups = results
                .GroupBy(r => GetDifficultyLevel(r.Width, r.Height, r.Mines))
                .ToList();

            foreach (var group in difficultyGroups)
            {
                grouped[group.Key] = group
                    .OrderBy(r => r.Duration)
                    .Take(10) // Топ 10 для кожної складності
                    .ToList();
            }

            return grouped;
        }

        private string GetDifficultyLevel(int width, int height, int mines)
        {
            var totalCells = width * height;
            var minePercent = (double)mines / totalCells * 100;

            if (totalCells <= 100)
                return $"Малий ({width}x{height}, {mines} мін)";
            else if (totalCells <= 400)
                return $"Середній ({width}x{height}, {mines} мін)";
            else
                return $"Великий ({width}x{height}, {mines} мін)";
        }
    }
}
