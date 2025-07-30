using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameResultService
    {
        Task SaveResultAsync(GameBoard board, string playerName, DateTime startTime, DateTime endTime);
        Task<List<GameResult>> GetResultsAsync();
    }
}
