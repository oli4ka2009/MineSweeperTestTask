using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameBoardFactory : IGameBoardFactory
    {
        public GameBoard Create(int width, int height, int minesCount)
        {
            var board = new GameBoard { Width = width, Height = height, MinesCount = minesCount };

            // 1. Створюємо пусте поле
            for (int i = 0; i < height; i++)
            {
                board.Cells.Add(new List<Cell>());
                for (int j = 0; j < width; j++)
                {
                    board.Cells[i].Add(new Cell());
                }
            }

            // 2. Розставляємо міни
            var random = new Random();
            int minesPlaced = 0;
            while (minesPlaced < minesCount)
            {
                int row = random.Next(height);
                int col = random.Next(width);
                if (!board.Cells[row][col].IsMine)
                {
                    board.Cells[row][col].IsMine = true;
                    minesPlaced++;
                }
            }

            // 3. Рахуємо сусідні міни для кожної клітинки
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (board.Cells[i][j].IsMine) continue;

                    int adjacentMines = 0;
                    for (int di = -1; di <= 1; di++)
                    {
                        for (int dj = -1; dj <= 1; dj++)
                        {
                            if (di == 0 && dj == 0) continue;
                            int ni = i + di;
                            int nj = j + dj;
                            if (ni >= 0 && ni < height && nj >= 0 && nj < width && board.Cells[ni][nj].IsMine)
                            {
                                adjacentMines++;
                            }
                        }
                    }
                    board.Cells[i][j].AdjacentMines = adjacentMines;
                }
            }
            return board;
        }
    }
}
