using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameService
    {
        void RevealCell(GameBoard board, int row, int col);
        void ToggleFlag(GameBoard board, int row, int col);
        bool CheckForWin(GameBoard board);
    }
}
