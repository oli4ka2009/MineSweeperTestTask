using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameFactory
    {
        GameBoard StartNewGame(int width, int height, int minesCount);
    }
}
