using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameplayService : IGameplayService
    {
        private readonly ICellInteractionService _cellInteractionService;
        private readonly IGameStateEvaluator _gameStateEvaluator;

        // 2. Створюємо конструктор, який приймає потрібні сервіси
        public GameplayService(ICellInteractionService cellInteractionService, IGameStateEvaluator gameStateEvaluator)
        {
            _cellInteractionService = cellInteractionService;
            _gameStateEvaluator = gameStateEvaluator;
        }

        public void HandlePlayerMove(GameBoard board, int row, int col, string mode)
        {
            if (mode == "flag")
            {
                _cellInteractionService.ToggleFlag(board, row, col);
            }
            else
            {
                _cellInteractionService.RevealCell(board, row, col);
                if (!board.IsGameOver && _gameStateEvaluator.CheckForWin(board))
                {
                    board.IsGameWon = true;
                }
            }
        }
    }
}
