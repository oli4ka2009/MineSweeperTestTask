using MineSweeper.Configuration;
using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class DifficultyAnalyzer : IDifficultyAnalyzer
    {
        private readonly DifficultySettings _settings;

        public DifficultyAnalyzer()
        {
            _settings = new DifficultySettings
            {
                SmallBoardLimit = 100,
                MediumBoardLimit = 400,
                TopResultsCount = 10
            };
        }

        public string GetDifficultyLevel(int width, int height, int mines)
        {
            var totalCells = width * height;

            if (totalCells <= _settings.SmallBoardLimit)
                return $"Малий ({width}x{height}, {mines} мін)";

            if (totalCells <= _settings.MediumBoardLimit)
                return $"Середній ({width}x{height}, {mines} мін)";

            return $"Великий ({width}x{height}, {mines} мін)";
        }

        public Dictionary<string, List<GameResult>> GroupResultsByDifficulty(List<GameResult> results, int topCount = 10)
        {
            var grouped = new Dictionary<string, List<GameResult>>();

            var difficultyGroups = results
                .GroupBy(r => GetDifficultyLevel(r.Width, r.Height, r.Mines))
                .ToList();

            foreach (var group in difficultyGroups)
            {
                grouped[group.Key] = group
                    .OrderBy(r => r.Duration)
                    .ThenByDescending(r => r.EndTime)
                    .Take(topCount)
                    .ToList();
            }

            return grouped;
        }
    }
}
