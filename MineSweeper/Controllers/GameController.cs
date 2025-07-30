using Microsoft.AspNetCore.Mvc;
using MineSweeper.Constants;
using MineSweeper.Extensions;
using MineSweeper.Models;
using MineSweeper.Models.ViewModels;
using MineSweeper.Services;

namespace MineSweeper.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameBoardFactory _boardFactory;
        private readonly IMinesweeperSolver _solverService;
        private readonly IGameSessionService _sessionService;
        private readonly IGameResultService _resultService;
        private readonly IGameFactory _gameFactory;
        private readonly IGameStateEvaluator _gameStateEvaluator;
        private readonly IGameplayService _gameplayService;
        private readonly IHintService _hintService;

        public GameController(IGameBoardFactory boardFactory, 
            IMinesweeperSolver solverService, IGameSessionService sessionService, 
            IGameResultService resultService, IGameFactory gameFactory, 
            IGameStateEvaluator gameStateEvaluator, IGameplayService gameplayService, IHintService hintService)
        {
            _boardFactory = boardFactory;
            _solverService = solverService;
            _sessionService = sessionService;
            _resultService = resultService;
            _gameFactory = gameFactory;
            _gameStateEvaluator = gameStateEvaluator;
            _gameplayService = gameplayService;
            _hintService = hintService;
        }

        public ActionResult Index()
        {
            var model = new NewGameViewModel
            {
                Width = 10,
                Height = 10,
                Mines = 15
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NewGameViewModel model)
        {
            if (!ModelState.IsValid || model.Mines >= model.Width * model.Height)
            {
                if (model.Mines >= model.Width * model.Height)
                    ModelState.AddModelError("Mines", "Кількість мін має бути меншою за кількість клітинок.");
                return View(model);
            }

            _sessionService.SavePlayerName(HttpContext, model.PlayerName);
            var board = _gameFactory.StartNewGame(model.Width, model.Height, model.Mines);
            _sessionService.SaveGameBoard(HttpContext, board);
            _sessionService.SaveGameMode(HttpContext, "reveal");

            return RedirectToAction("Play");
        }

        public IActionResult Play()
        {
            var board = _sessionService.LoadGameBoard(HttpContext);

            if (board == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new PlayViewModel
            {
                Board = board,
                PlayerName = _sessionService.GetPlayerName(HttpContext),
                MinesLeft = _gameStateEvaluator.CountRemainingMines(board)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> HandleClickAsync(int row, int col)
        {
            var board = _sessionService.LoadGameBoard(HttpContext);
            var gameMode = _sessionService.GetGameMode(HttpContext);

            if (board == null || board.IsGameOver)
            {
                _sessionService.ClearGameStartTime(HttpContext);
                return RedirectToAction("Index");
            }

            if (_sessionService.GetGameStartTime(HttpContext) == null)
            {
                _sessionService.SaveGameStartTime(HttpContext);
            }

            _gameplayService.HandlePlayerMove(board, row, col, gameMode);

            await CheckAndSaveGameResult(board);

            _sessionService.SaveGameBoard(HttpContext, board);

            return RedirectToAction("Play");
        }

        [HttpPost]
        public IActionResult SetMode(string currentMode)
        {
            _sessionService.SaveGameMode(HttpContext, currentMode ?? "reveal");
            return RedirectToAction("Play");
        }

        [HttpPost]
        public async Task<IActionResult> SolveNextStepAsync()
        {
            var board = _sessionService.LoadGameBoard(HttpContext);

            if (board == null || board.IsGameOver)
            {
                _sessionService.ClearGameStartTime(HttpContext);
                return RedirectToAction("Play");
            }

            if (_sessionService.GetGameStartTime(HttpContext) == null)
            {
                _sessionService.SaveGameStartTime(HttpContext);
            }

            if (_hintService.SolveNextStep(board, _solverService))
            {
                await CheckAndSaveGameResult(board);

                _sessionService.SaveGameBoard(HttpContext, board);
            }

            return RedirectToAction("Play");
        }

        private async Task CheckAndSaveGameResult(GameBoard board)
        {
            if (board.IsGameWon)
            {
                var startTime = _sessionService.GetGameStartTime(HttpContext);
                var playerName = _sessionService.GetPlayerName(HttpContext);
                var endTime = DateTime.UtcNow;

                if (startTime.HasValue)
                {
                    try
                    {
                        await _resultService.SaveResultAsync(board, playerName, startTime.Value, endTime);
                        _sessionService.ClearGameStartTime(HttpContext);
                    }
                    catch (Exception ex)
                    {
                        _sessionService.ClearGameStartTime(HttpContext);
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult PlayAgain()
        {
            var oldBoard = _sessionService.LoadGameBoard(HttpContext);
            if (oldBoard == null)
            {
                return RedirectToAction("Index");
            }

            var board = _gameFactory.StartNewGame(oldBoard.Width, oldBoard.Height, oldBoard.MinesCount);

            _sessionService.SaveGameBoard(HttpContext, board);

            _sessionService.ClearGameStartTime(HttpContext);

            return RedirectToAction("Play");
        }
    }
}