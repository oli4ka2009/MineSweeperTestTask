namespace MineSweeper.Models.ViewModels
{
    public class LeaderboardViewModel
    {
        public List<GameResult> AllResults { get; set; } = new List<GameResult>();
        public Dictionary<string, List<GameResult>> BestResultsByDifficulty { get; set; } = new Dictionary<string, List<GameResult>>();

        public int TotalGamesPlayed => AllResults.Count;
        public int TotalPlayersCount => AllResults.Select(r => r.PlayerName).Distinct().Count();
        public TimeSpan? BestTimeOverall => AllResults.Any() ? AllResults.Min(r => r.Duration) : null;
        public string? BestPlayerOverall => AllResults.Any() ? AllResults.OrderBy(r => r.Duration).First().PlayerName : null;
    }
}
