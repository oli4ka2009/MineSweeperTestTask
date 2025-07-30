using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface ICellInteractionService
    {
        void RevealCell(GameBoard board, int row, int col);
        void ToggleFlag(GameBoard board, int row, int col);
    }
}
