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
            return await _context.GameResults
                .OrderBy(r => r.EndTime - r.StartTime)
                .ToListAsync();
        }
    }
}
