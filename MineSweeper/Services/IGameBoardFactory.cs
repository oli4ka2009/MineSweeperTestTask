using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameBoardFactory
    {
        GameBoard Create(int width, int height, int minesCount);
    }
}
