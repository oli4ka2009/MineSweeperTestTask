using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameStateEvaluator
    {
        bool CheckForWin(GameBoard board);
        int CountRemainingMines(GameBoard board);
    }
}
