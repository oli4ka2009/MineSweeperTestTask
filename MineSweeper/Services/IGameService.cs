using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameService
    {
        void RevealCell(GameBoard board, int row, int col);
        void ToggleFlag(GameBoard board, int row, int col);
        bool CheckForWin(GameBoard board);

        GameBoard StartNewGame(int width, int height, int minesCount);
        void HandlePlayerMove(GameBoard board, int row, int col, string mode);
        bool SolveNextStep(GameBoard board, IMinesweeperSolver solver);
        int CountRemainingMines(GameBoard board);
    }
}
