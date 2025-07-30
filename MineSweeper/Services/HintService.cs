using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class HintService : IHintService
    {
        private readonly ICellInteractionService _cellInteractionService;
        private readonly IGameStateEvaluator _gameStateEvaluator;

        public HintService(ICellInteractionService cellInteractionService, IGameStateEvaluator gameStateEvaluator)
        {
            _cellInteractionService = cellInteractionService;
            _gameStateEvaluator = gameStateEvaluator;
        }

        public bool SolveNextStep(GameBoard board, IMinesweeperSolver solver)
        {
            bool isFirstMove = !board.Cells.SelectMany(r => r).Any(c => c.IsRevealed);
            SolverMove? move;

            if (isFirstMove)
            {
                var random = new Random();
                int row = random.Next(board.Height);
                int col = random.Next(board.Width);
                move = new SolverMove { Row = row, Col = col, Action = MoveAction.Reveal };
            }
            else
            {
                move = solver.FindNextMove(board);

                if (move == null)
                {
                    move = solver.FindBestGuess(board);
                }
            }

            if (move == null)
            {
                return false;
            }

            if (move.Action == MoveAction.Flag)
                _cellInteractionService.ToggleFlag(board, move.Row, move.Col);
            else
                _cellInteractionService.RevealCell(board, move.Row, move.Col);

            if (!board.IsGameOver && _gameStateEvaluator.CheckForWin(board))
            {
                board.IsGameWon = true;
            }

            return true;
        }
    }
}
