namespace MineSweeper.Models
{
    public class GameBoard
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int MinesCount { get; set; }
        public List<List<Cell>> Cells { get; set; }
        public bool IsGameOver { get; set; } = false;
        public bool IsGameWon { get; set; } = false;

        // Конструктор можна залишити порожнім або додати логіку ініціалізації
        public GameBoard()
        {
            Cells = new List<List<Cell>>();
        }
    }
}
