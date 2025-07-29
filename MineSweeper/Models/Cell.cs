namespace MineSweeper.Models
{
    public class Cell
    {
        public bool IsMine { get; set; } = false;
        public bool IsRevealed { get; set; } = false;
        public bool IsFlagged { get; set; } = false;
        public int AdjacentMines { get; set; } = 0;
    }
}
