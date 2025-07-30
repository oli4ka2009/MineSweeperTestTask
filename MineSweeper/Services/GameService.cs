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

        public GameBoard StartNewGame(int width, int height, int minesCount)
        {
            return new GameBoardFactory().Create(width, height, minesCount);
        }

        public void HandlePlayerMove(GameBoard board, int row, int col, string mode)
        {
            if (mode == "flag")
            {
                ToggleFlag(board, row, col);
            }
            else
            {
                RevealCell(board, row, col);
                if (!board.IsGameOver && CheckForWin(board))
                {
                    board.IsGameWon = true;
                }
            }
        }

        public bool SolveNextStep(GameBoard board, IMinesweeperSolver solver)
        {
            bool isFirstMove = !board.Cells.SelectMany(r => r).Any(c => c.IsRevealed);
            SolverMove? move;

            if (isFirstMove)
            {
                var random = new Random();
                int row = random.Next(board.Height);
                int col = random.Next(board.Width);
                move = new SolverMove { Row = row, Col = col, Action = MoveAction.Reveal };
            }
            else
            {
                move = solver.FindNextMove(board);

                if (move == null)
                {
                    move = solver.FindBestGuess(board);
                }
            }

            if (move == null)
            {
                return false;
            }

            if (move.Action == MoveAction.Flag)
                ToggleFlag(board, move.Row, move.Col);
            else
                RevealCell(board, move.Row, move.Col);

            if (!board.IsGameOver && CheckForWin(board))
            {
                board.IsGameWon = true;
            }

            return true;
        }

        public int CountRemainingMines(GameBoard board)
        {
            int flagsPlaced = board.Cells.SelectMany(row => row).Count(cell => cell.IsFlagged);
            return board.MinesCount - flagsPlaced;
        }
    }
}
