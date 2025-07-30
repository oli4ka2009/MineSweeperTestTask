using MineSweeper.Models.ViewModels;

namespace MineSweeper.Services
{
    public interface ILeaderboardService
    {
        Task<LeaderboardViewModel> GetLeaderboardDataAsync();
    }
}
