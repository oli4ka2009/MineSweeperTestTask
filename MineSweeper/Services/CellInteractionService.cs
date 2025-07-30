using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class CellInteractionService : ICellInteractionService
    {
        public void RevealCell(GameBoard board, int row, int col)
        {
            if (row < 0 || row >= board.Height || col < 0 || col >= board.Width) return;

            var cell = board.Cells[row][col];

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
            if (!cell.IsRevealed)
            {
                cell.IsFlagged = !cell.IsFlagged;
            }
        }
    }
}
