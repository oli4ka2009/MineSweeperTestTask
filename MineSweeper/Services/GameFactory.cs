using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameFactory : IGameFactory
    {
        public GameBoard StartNewGame(int width, int height, int minesCount)
        {
            return new GameBoardFactory().Create(width, height, minesCount);
        }
    }
}
