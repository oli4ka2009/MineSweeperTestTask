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
        private readonly IGameService _gameService;
        private readonly IMinesweeperSolver _solverService;
        private readonly IGameSessionService _sessionService;
        private readonly IGameResultService _resultService;

        public GameController(IGameBoardFactory boardFactory, IGameService gameService, IMinesweeperSolver solverService, IGameSessionService sessionService, IGameResultService resultService)
        {
            _boardFactory = boardFactory;
            _gameService = gameService;
            _solverService = solverService;
            _sessionService = sessionService;
            _resultService = resultService;
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
            var board = _gameService.StartNewGame(model.Width, model.Height, model.Mines);
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
                MinesLeft = _gameService.CountRemainingMines(board)
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

            _gameService.HandlePlayerMove(board, row, col, gameMode);
            _sessionService.SaveGameBoard(HttpContext, board);

            if (board.IsGameWon)
            {
                var startTime = _sessionService.GetGameStartTime(HttpContext);
                var playerName = _sessionService.GetPlayerName(HttpContext);
                var endTime = DateTime.UtcNow;

                if (startTime.HasValue)
                {
                    await _resultService.SaveResultAsync(board, playerName, startTime.Value, endTime);
                    _sessionService.ClearGameStartTime(HttpContext);
                }
            }

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

            if (board.IsGameWon)
            {
                var startTime = _sessionService.GetGameStartTime(HttpContext);
                var playerName = _sessionService.GetPlayerName(HttpContext);
                var endTime = DateTime.UtcNow;

                if (startTime.HasValue)
                {
                    await _resultService.SaveResultAsync(board, playerName, startTime.Value, endTime);
                    _sessionService.ClearGameStartTime(HttpContext);
                    return RedirectToAction("Play");
                }
            }

            if (_sessionService.GetGameStartTime(HttpContext) == null)
            {
                _sessionService.SaveGameStartTime(HttpContext);
            }

            if (_gameService.SolveNextStep(board, _solverService))
            {
                _sessionService.SaveGameBoard(HttpContext, board);
            }

            return RedirectToAction("Play");
        }
    }
}