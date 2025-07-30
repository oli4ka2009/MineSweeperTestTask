using Microsoft.AspNetCore.Mvc;

using MineSweeper.Models.ViewModels;

using MineSweeper.Services;



namespace MineSweeper.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _leaderboardService.GetLeaderboardDataAsync();
            return View(viewModel);
        }
    }
}