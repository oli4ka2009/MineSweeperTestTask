using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameService : IGameService
    {
        public void RevealCell(GameBoard board, int row, int col)
        {
            // Перевірка виходу за межі поля
            if (row < 0 || row >= board.Height || col < 0 || col >= board.Width) return;

            var cell = board.Cells[row][col];

            // Не відкриваємо вже відкриту клітинку або клітинку з прапорцем
            if (cell.IsRevealed || cell.IsFlagged) return;

            cell.IsRevealed = true;

            if (cell.IsMine)
            {
                board.IsGameOver = true;
                return;
            }

            if (cell.AdjacentMines == 0)
            {
                for (int di = -1; di <= 1; di++)
                {
                    for (int dj = -1; dj <= 1; dj++)
                    {
                        if (di == 0 && dj == 0) continue;
                        RevealCell(board, row + di, col + dj);
                    }
                }
            }
        }

        public void ToggleFlag(GameBoard board, int row, int col)
        {
            if (row < 0 || row >= board.Height || col < 0 || col >= board.Width) return;
            var cell = board.Cells[row][col];
            if (!cell.IsRevealed) // Прапорець можна ставити/знімати тільки на закритих клітинках
            {
                cell.IsFlagged = !cell.IsFlagged;
            }
        }

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
    }
}
