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

        public GameController(IGameBoardFactory boardFactory, IGameService gameService, IMinesweeperSolver solverService)
        {
            _boardFactory = boardFactory;
            _gameService = gameService;
            _solverService = solverService;
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
            if (ModelState.IsValid)
            {
                if (model.Mines >= model.Width * model.Height)
                {
                    ModelState.AddModelError("Mines", "Кількість мін має бути меншою за кількість клітинок.");
                    return View(model);
                }

                HttpContext.Session.SetString(SessionKeys.PlayerName, model.PlayerName);

                GameBoard board = _boardFactory.Create(model.Width, model.Height, model.Mines);

                HttpContext.Session.SetObject(SessionKeys.GameBoard, board);
                HttpContext.Session.SetString(SessionKeys.GameMode, "reveal");

                // Очищуємо час старту - він буде встановлений при першому ході
                HttpContext.Session.Remove(SessionKeys.GameStartTime);

                return RedirectToAction("Play");
            }

            return View(model);
        }

        public IActionResult Play()
        {
            var board = HttpContext.Session.GetObject<GameBoard>(SessionKeys.GameBoard);

            if (board == null)
            {
                return RedirectToAction("Index");
            }

            // Розраховуємо кількість мін, що залишилися
            int flagsPlaced = board.Cells.SelectMany(row => row).Count(cell => cell.IsFlagged);
            int minesLeft = board.MinesCount - flagsPlaced;

            // Отримуємо час старту (якщо є)
            var startTime = HttpContext.Session.GetObject<DateTime?>(SessionKeys.GameStartTime);

            var viewModel = new PlayViewModel
            {
                Board = board,
                PlayerName = HttpContext.Session.GetString(SessionKeys.PlayerName) ?? "Гравець",
                MinesLeft = minesLeft,
                GameStartTime = startTime, // Передаємо час старту в ViewModel
                IsGameActive = !board.IsGameOver && !board.IsGameWon && startTime.HasValue
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult HandleClick(int row, int col)
        {
            var board = HttpContext.Session.GetObject<GameBoard>(SessionKeys.GameBoard);
            var gameMode = HttpContext.Session.GetString(SessionKeys.GameMode) ?? "reveal";

            if (board == null || board.IsGameOver)
            {
                return RedirectToAction("Index");
            }

            // Встановлюємо час старту при першому ході
            if (HttpContext.Session.GetObject<DateTime?>(SessionKeys.GameStartTime) == null)
            {
                HttpContext.Session.SetObject(SessionKeys.GameStartTime, DateTime.UtcNow);
            }

            if (gameMode == "flag")
            {
                _gameService.ToggleFlag(board, row, col);
            }
            else
            {
                _gameService.RevealCell(board, row, col);
                if (!board.IsGameOver)
                {
                    if (_gameService.CheckForWin(board))
                    {
                        board.IsGameWon = true;
                    }
                }
            }

            HttpContext.Session.SetObject(SessionKeys.GameBoard, board);
            return RedirectToAction("Play");
        }

        [HttpPost]
        public IActionResult SetMode(string currentMode)
        {
            HttpContext.Session.SetString(SessionKeys.GameMode, currentMode ?? "reveal");
            return RedirectToAction("Play");
        }

        [HttpPost]
        public IActionResult SolveNextStep()
        {
            var board = HttpContext.Session.GetObject<GameBoard>("GameBoard");
            if (board == null || board.IsGameOver || board.IsGameWon)
            {
                return RedirectToAction("Play");
            }

            // Встановлюємо час старту, якщо це перший хід
            if (HttpContext.Session.GetObject<DateTime?>(SessionKeys.GameStartTime) == null)
            {
                HttpContext.Session.SetObject(SessionKeys.GameStartTime, DateTime.UtcNow);
            }

            bool isFirstMove = !board.Cells.SelectMany(r => r).Any(c => c.IsRevealed);
            SolverMove? move = null;

            if (isFirstMove)
            {
                var random = new Random();
                int row = random.Next(board.Height);
                int col = random.Next(board.Width);
                move = new SolverMove { Row = row, Col = col, Action = MoveAction.Reveal };
            }
            else
            {
                move = _solverService.FindNextMove(board);

                if (move == null)
                {
                    TempData["SolverMessage"] = "Логічного ходу не знайдено, бот вгадує.";
                    move = _solverService.FindBestGuess(board);
                }
            }

            if (move != null)
            {
                if (move.Action == MoveAction.Flag)
                {
                    _gameService.ToggleFlag(board, move.Row, move.Col);
                }
                else
                {
                    _gameService.RevealCell(board, move.Row, move.Col);
                }

                if (!board.IsGameOver && _gameService.CheckForWin(board))
                {
                    board.IsGameWon = true;
                }
            }
            else
            {
                TempData["SolverMessage"] = "Бот не може зробити хід (можливо, гра завершена).";
            }

            HttpContext.Session.SetObject("GameBoard", board);
            return RedirectToAction("Play");
        }
    }
}