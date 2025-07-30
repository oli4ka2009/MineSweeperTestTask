using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IHintService
    {
        bool SolveNextStep(GameBoard board, IMinesweeperSolver solver);
    }
}
