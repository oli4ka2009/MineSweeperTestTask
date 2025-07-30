using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class MinesweeperSolver : IMinesweeperSolver
    {
        public SolverMove? FindNextMove(GameBoard board)
        {
            var basicMove = FindBasicMove(board);
            if (basicMove != null)
            {
                return basicMove;
            }

            var advancedMove = FindAdvancedMove(board);
            if (advancedMove != null)
            {
                return advancedMove;
            }

            return null;
        }

        public SolverMove? FindBestGuess(GameBoard board)
        {
            var availableCells = new List<(int row, int col)>();
            for (int i = 0; i < board.Height; i++)
            {
                for (int j = 0; j < board.Width; j++)
                {
                    if (!board.Cells[i][j].IsRevealed && !board.Cells[i][j].IsFlagged)
                    {
                        availableCells.Add((i, j));
                    }
                }
            }

            if (availableCells.Any())
            {
                var random = new Random();
                var (row, col) = availableCells[random.Next(availableCells.Count)];
                return new SolverMove { Row = row, Col = col, Action = MoveAction.Reveal };
            }

            return null;
        }

        private SolverMove? FindBasicMove(GameBoard board)
        {
            for (int i = 0; i < board.Height; i++)
            {
                for (int j = 0; j < board.Width; j++)
                {
                    var cell = board.Cells[i][j];
                    if (!cell.IsRevealed || cell.AdjacentMines == 0) continue;

                    var neighbors = GetNeighbors(board, i, j);
                    var hiddenNeighbors = neighbors.Where(n => !n.cell.IsRevealed).ToList();
                    var flaggedNeighbors = neighbors.Where(n => n.cell.IsFlagged).ToList();

                    if (cell.AdjacentMines == flaggedNeighbors.Count && hiddenNeighbors.Count > flaggedNeighbors.Count)
                    {
                        var safeCell = hiddenNeighbors.FirstOrDefault(n => !n.cell.IsFlagged);
                        if (safeCell != default)
                        {
                            return new SolverMove { Row = safeCell.row, Col = safeCell.col, Action = MoveAction.Reveal };
                        }
                    }

                    if (cell.AdjacentMines == hiddenNeighbors.Count && hiddenNeighbors.Count > flaggedNeighbors.Count)
                    {
                        var mineCell = hiddenNeighbors.FirstOrDefault(n => !n.cell.IsFlagged);
                        if (mineCell != default)
                        {
                            return new SolverMove { Row = mineCell.row, Col = mineCell.col, Action = MoveAction.Flag };
                        }
                    }
                }
            }
            return null;
        }

        private SolverMove? FindAdvancedMove(GameBoard board)
        {
            var revealedCells = GetRevealedNumberedCells(board);

            foreach (var (cell1, r1, c1) in revealedCells)
            {
                int effectiveMines1 = cell1.AdjacentMines - GetFlaggedNeighbors(board, r1, c1).Count;
                var unflaggedNeighbors1 = GetHiddenNeighbors(board, r1, c1).Where(n => !n.cell.IsFlagged).ToList();

                if (unflaggedNeighbors1.Count == 0 || effectiveMines1 <= 0) continue;

                foreach (var (cell2, r2, c2) in revealedCells)
                {
                    if (r1 == r2 && c1 == c2) continue;

                    int effectiveMines2 = cell2.AdjacentMines - GetFlaggedNeighbors(board, r2, c2).Count;
                    var unflaggedNeighbors2 = GetHiddenNeighbors(board, r2, c2).Where(n => !n.cell.IsFlagged).ToList();

                    if (unflaggedNeighbors2.Count <= unflaggedNeighbors1.Count || effectiveMines2 <= 0) continue;

                    if (unflaggedNeighbors1.All(n1 => unflaggedNeighbors2.Any(n2 => n1.row == n2.row && n1.col == n2.col)))
                    {
                        var uniqueToNeighbors2 = unflaggedNeighbors2.Where(n2 => !unflaggedNeighbors1.Any(n1 => n1.row == n2.row && n1.col == n2.col)).ToList();
                        int mineDifference = effectiveMines2 - effectiveMines1;

                        if (uniqueToNeighbors2.Any() && mineDifference == uniqueToNeighbors2.Count)
                        {
                            var move = uniqueToNeighbors2.First();
                            return new SolverMove { Row = move.row, Col = move.col, Action = MoveAction.Flag };
                        }

                        if (uniqueToNeighbors2.Any() && mineDifference == 0)
                        {
                            var move = uniqueToNeighbors2.First();
                            return new SolverMove { Row = move.row, Col = move.col, Action = MoveAction.Reveal };
                        }
                    }
                }
            }
            return null;
        }

        private List<(Cell cell, int row, int col)> GetNeighbors(GameBoard board, int row, int col)
        {
            var result = new List<(Cell, int, int)>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int newRow = row + i;
                    int newCol = col + j;
                    if (newRow >= 0 && newRow < board.Height && newCol >= 0 && newCol < board.Width)
                    {
                        result.Add((board.Cells[newRow][newCol], newRow, newCol));
                    }
                }
            }
            return result;
        }

        private List<(Cell cell, int row, int col)> GetRevealedNumberedCells(GameBoard board)
        {
            var result = new List<(Cell cell, int row, int col)>();
            for (int i = 0; i < board.Height; i++)
            {
                for (int j = 0; j < board.Width; j++)
                {
                    if (board.Cells[i][j].IsRevealed && board.Cells[i][j].AdjacentMines > 0)
                    {
                        result.Add((board.Cells[i][j], i, j));
                    }
                }
            }
            return result;
        }

        private List<(Cell cell, int row, int col)> GetHiddenNeighbors(GameBoard b, int r, int c) => GetNeighbors(b, r, c).Where(n => !n.cell.IsRevealed).ToList();
        private List<(Cell cell, int row, int col)> GetFlaggedNeighbors(GameBoard b, int r, int c) => GetNeighbors(b, r, c).Where(n => n.cell.IsFlagged).ToList();
    }
}
