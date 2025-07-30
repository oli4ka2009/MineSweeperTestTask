namespace MineSweeper.Models
{
    public class GameResult
    {
        public int Id { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Mines { get; set; }

        public TimeSpan Duration => EndTime - StartTime;
    }
}
