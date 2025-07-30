using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameplayService
    {
        void HandlePlayerMove(GameBoard board, int row, int col, string mode);
    }
}
