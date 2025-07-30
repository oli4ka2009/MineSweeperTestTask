using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IMinesweeperSolver
    {
        SolverMove FindNextMove(GameBoard board);
        SolverMove? FindBestGuess(GameBoard board);
    }
}
