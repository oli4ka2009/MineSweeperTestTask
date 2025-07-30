namespace MineSweeper.Models.ViewModels
{
    public class PlayViewModel
    {
        public GameBoard Board { get; set; }
        public string PlayerName { get; set; }
        public int MinesLeft { get; set; }
    }
}