using Microsoft.AspNetCore.Mvc;
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

                HttpContext.Session.SetString("PlayerName", model.PlayerName);

                GameBoard board = _boardFactory.Create(model.Width, model.Height, model.Mines);

                HttpContext.Session.SetObject("GameBoard", board);
                HttpContext.Session.SetString("GameMode", "reveal");
                HttpContext.Session.Remove("GameStartTime");
                return RedirectToAction("Play");
            }

            // Якщо є помилки валідації, повертаємо користувачу ту саму форму,
            // але вже з повідомленнями про помилки.
            return View(model);
        }

        // Дія для відображення ігрового поля
        public ActionResult Play()
        {
            var board = HttpContext.Session.GetObject<GameBoard>("GameBoard");
            if (board == null)
            {
                return RedirectToAction("Index"); // Якщо гри немає, повертаємо на старт
            }
            return View(board);
        }

        [HttpPost]
        public IActionResult HandleClick(int row, int col)
        {
            var board = HttpContext.Session.GetObject<GameBoard>("GameBoard");
            var gameMode = HttpContext.Session.GetString("GameMode") ?? "reveal";

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

            if (HttpContext.Session.GetObject<DateTime?>("GameStartTime") == null)
            {
                // Зберігаємо поточний час як час початку
                HttpContext.Session.SetObject("GameStartTime", DateTime.UtcNow);
            }
            // Тут буде логіка перевірки на перемогу/поразку

            HttpContext.Session.SetObject("GameBoard", board); // Зберігаємо оновлене поле
            return RedirectToAction("Play"); // Перенаправляємо назад на ігрове поле
        }

        [HttpPost]
        public IActionResult SetMode(string currentMode)
        {
            // Зберігаємо вибраний режим у сесію
            HttpContext.Session.SetString("GameMode", currentMode ?? "reveal");

            // Повертаємо користувача на ігрове поле
            return RedirectToAction("Play");
        }
    }
}
