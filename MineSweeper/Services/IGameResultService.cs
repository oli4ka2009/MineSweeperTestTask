using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameResultService
    {
        Task SaveResultAsync(GameBoard board, string playerName, DateTime startTime, DateTime endTime);
        Task<List<GameResult>> GetResultsAsync();
        Task<List<GameResult>> GetTopResultsForDifficultyAsync(int width, int height, int mines, int count = 10);
        Task<List<GameResult>> GetPlayerBestResultsAsync(string playerName, int count = 10);
        Task<PlayerStats> GetPlayerStatsAsync(string playerName);
    }
}
