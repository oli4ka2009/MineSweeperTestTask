namespace MineSweeper.Models
{
    public class PlayerStats
    {
        public string PlayerName { get; set; } = string.Empty;
        public int TotalGames { get; set; }
        public TimeSpan BestTime { get; set; }
        public TimeSpan AverageTime { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}
