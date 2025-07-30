using Microsoft.EntityFrameworkCore;
using MineSweeper.Data;
using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameResultService : IGameResultService
    {
        private readonly ApplicationDbContext _context;

        public GameResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveResultAsync(GameBoard board, string playerName, DateTime startTime, DateTime endTime)
        {
            var result = new GameResult
            {
                PlayerName = playerName,
                StartTime = startTime,
                EndTime = endTime,
                Width = board.Width,
                Height = board.Height,
                Mines = board.MinesCount
            };
            _context.GameResults.Add(result);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GameResult>> GetResultsAsync()
        {
            var results = await _context.GameResults.ToListAsync();
            return results
                .OrderBy(r => r.Duration)
                .ThenByDescending(r => r.EndTime)
                .ToList();
        }

        public async Task<List<GameResult>> GetTopResultsForDifficultyAsync(int width, int height, int mines, int count = 10)
        {
            var results = await _context.GameResults
                .Where(r => r.Width == width && r.Height == height && r.Mines == mines)
                .ToListAsync();

            return results
                .OrderBy(r => r.Duration)
                .ThenByDescending(r => r.EndTime)
                .Take(count)
                .ToList();
        }

        public async Task<List<GameResult>> GetPlayerBestResultsAsync(string playerName, int count = 10)
        {
            var results = await _context.GameResults
                .Where(r => r.PlayerName == playerName)
                .ToListAsync();

            return results
                .OrderBy(r => r.Duration)
                .ThenByDescending(r => r.EndTime)
                .Take(count)
                .ToList();
        }

        public async Task<PlayerStats> GetPlayerStatsAsync(string playerName)
        {
            var results = await _context.GameResults
                .Where(r => r.PlayerName == playerName)
                .ToListAsync();

            if (!results.Any())
            {
                return new PlayerStats { PlayerName = playerName };
            }

            return new PlayerStats
            {
                PlayerName = playerName,
                TotalGames = results.Count,
                BestTime = results.Min(r => r.Duration),
                AverageTime = TimeSpan.FromMilliseconds(results.Average(r => r.Duration.TotalMilliseconds)),
                LastPlayed = results.Max(r => r.EndTime)
            };
        }
    }
}