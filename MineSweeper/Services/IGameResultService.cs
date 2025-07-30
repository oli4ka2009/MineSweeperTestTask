using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameResultService
    {
        Task SaveResultAsync(GameBoard board, string playerName, DateTime startTime, DateTime endTime);
        public Task<List<GameResult>> GetResultsAsync();
        public Task<List<GameResult>> GetTopResultsForDifficultyAsync(int width, int height, int mines, int count = 10);
        public Task<List<GameResult>> GetPlayerBestResultsAsync(string playerName, int count = 10);
        public Task<PlayerStats> GetPlayerStatsAsync(string playerName);
    }
}
