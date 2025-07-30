using MineSweeper.Models.ViewModels;

namespace MineSweeper.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IGameResultService _resultService;
        private readonly IDifficultyAnalyzer _difficultyAnalyzer;

        public LeaderboardService(IGameResultService resultService, IDifficultyAnalyzer difficultyAnalyzer)
        {
            _resultService = resultService;
            _difficultyAnalyzer = difficultyAnalyzer;
        }

        public async Task<LeaderboardViewModel> GetLeaderboardDataAsync()
        {
            var results = await _resultService.GetResultsAsync();

            return new LeaderboardViewModel
            {
                AllResults = results,
                BestResultsByDifficulty = _difficultyAnalyzer.GroupResultsByDifficulty(results)
            };
        }
    }
}
