using Microsoft.AspNetCore.Http;
using MineSweeper.Constants;
using MineSweeper.Extensions;
using MineSweeper.Models;

namespace MineSweeper.Services
{
    public class GameSessionService : IGameSessionService
    {
        public void SaveGameBoard(HttpContext context, GameBoard board)
        => context.Session.SetObject(SessionKeys.GameBoard, board);

        public GameBoard? LoadGameBoard(HttpContext context)
            => context.Session.GetObject<GameBoard>(SessionKeys.GameBoard);

        public void SaveGameMode(HttpContext context, string mode)
            => context.Session.SetString(SessionKeys.GameMode, mode);

        public string GetGameMode(HttpContext context)
            => context.Session.GetString(SessionKeys.GameMode) ?? "reveal";

        public void SavePlayerName(HttpContext context, string name)
            => context.Session.SetString(SessionKeys.PlayerName, name);

        public string GetPlayerName(HttpContext context)
            => context.Session.GetString(SessionKeys.PlayerName) ?? "Гравець";

        public void SaveGameStartTime(HttpContext context)
        => context.Session.SetObject(SessionKeys.GameStartTime, DateTime.UtcNow);

        public DateTime? GetGameStartTime(HttpContext context)
            => context.Session.GetObject<DateTime?>(SessionKeys.GameStartTime);

        public void ClearGameStartTime(HttpContext context)
            => context.Session.Remove(SessionKeys.GameStartTime);
    }
}
