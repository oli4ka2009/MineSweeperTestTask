using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameStateEvaluator : IGameStateEvaluator
    {
        public bool CheckForWin(GameBoard board)
        {
            // Перемога, якщо всі не-міни відкрито
            for (int i = 0; i < board.Height; i++)
            {
                for (int j = 0; j < board.Width; j++)
                {
                    var cell = board.Cells[i][j];
                    if (!cell.IsMine && !cell.IsRevealed)
                    {
                        return false; // Знайдена закрита безпечна клітинка
                    }
                }
            }
            return true; // Всі безпечні клітинки відкрито
        }

        public int CountRemainingMines(GameBoard board)
        {
            int flagsPlaced = board.Cells.SelectMany(row => row).Count(cell => cell.IsFlagged);
            return board.MinesCount - flagsPlaced;
        }
    }
}
