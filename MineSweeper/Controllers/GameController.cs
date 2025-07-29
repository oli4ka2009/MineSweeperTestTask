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

        public GameController(IGameBoardFactory boardFactory, IGameService gameService)
        {
            _boardFactory = boardFactory;
            _gameService = gameService;
        }

        public ActionResult Index()
        {
            // Створюємо модель з деякими значеннями за замовчуванням
            var model = new NewGameViewModel
            {
                Width = 10,
                Height = 10,
                Mines = 15
            };
            return View(model);
        }

        // POST: /Game/Index
        // Ця дія спрацює, коли користувач натисне кнопку "Почати гру"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NewGameViewModel model)
        {
            // Перевіряємо, чи пройшла модель валідацію
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
                HttpContext.Session.Remove(SessionKeys.GameStartTime);
                return RedirectToAction("Play");
            }

            // Якщо є помилки валідації, повертаємо користувачу ту саму форму,
            // але вже з повідомленнями про помилки.
            return View(model);
        }

        // Дія для відображення ігрового поля
        public IActionResult Play()
        {
            // 1. Отримуємо дошку з сесії
            var board = HttpContext.Session.GetObject<GameBoard>(SessionKeys.GameBoard);

            // Якщо дошки в сесії немає (наприклад, сесія закінчилася),
            // перенаправляємо на сторінку створення нової гри.
            if (board == null)
            {
                return RedirectToAction("Index");
            }

            // 2. Розраховуємо кількість мін, що залишилися
            int flagsPlaced = board.Cells.SelectMany(row => row).Count(cell => cell.IsFlagged);
            int minesLeft = board.MinesCount - flagsPlaced;

            // 3. Розраховуємо час, що минув
            var startTime = HttpContext.Session.GetObject<DateTime?>(SessionKeys.GameStartTime);
            TimeSpan elapsedTime = TimeSpan.Zero;

            if (startTime.HasValue)
            {
                // Якщо гра ще триває, рахуємо час від старту до зараз.
                // Якщо гра закінчена, час "заморожується" на момент останнього ходу,
                // оскільки цей код виконується лише при перезавантаженні сторінки.
                elapsedTime = DateTime.UtcNow - startTime.Value;
            }

            // 4. Створюємо та заповнюємо ViewModel
            var viewModel = new PlayViewModel
            {
                Board = board,
                PlayerName = HttpContext.Session.GetString(SessionKeys.PlayerName) ?? "Гравець",
                MinesLeft = minesLeft,
                ElapsedTimeFormatted = elapsedTime.ToString(@"mm\:ss") // Форматуємо час
            };

            // 5. Передаємо готову ViewModel у представлення
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

            if (gameMode == "flag")
            {
                _gameService.ToggleFlag(board, row, col);
            }
            else // "reveal" mode
            {
                _gameService.RevealCell(board, row, col);
                if (!board.IsGameOver) // Перевіряємо на перемогу тільки якщо гра не закінчилася поразкою
                {
                    if (_gameService.CheckForWin(board))
                    {
                        board.IsGameWon = true;
                    }
                }
            }

            if (HttpContext.Session.GetObject<DateTime?>(SessionKeys.GameStartTime) == null)
            {
                // Зберігаємо поточний час як час початку
                HttpContext.Session.SetObject(SessionKeys.GameStartTime, DateTime.UtcNow);
            }
            // Тут буде логіка перевірки на перемогу/поразку

            HttpContext.Session.SetObject(SessionKeys.GameBoard, board); // Зберігаємо оновлене поле
            return RedirectToAction("Play"); // Перенаправляємо назад на ігрове поле
        }

        [HttpPost]
        public IActionResult SetMode(string currentMode)
        {
            // Зберігаємо вибраний режим у сесію
            HttpContext.Session.SetString(SessionKeys.GameMode, currentMode ?? "reveal");

            // Повертаємо користувача на ігрове поле
            return RedirectToAction("Play");
        }
    }
}
