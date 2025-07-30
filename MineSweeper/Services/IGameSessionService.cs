using MineSweeper.Models;

namespace MineSweeper.Services
{
    public interface IGameSessionService
    {
        void SaveGameBoard(HttpContext context, GameBoard board);
        GameBoard? LoadGameBoard(HttpContext context);

        void SaveGameMode(HttpContext context, string mode);
        string GetGameMode(HttpContext context);

        void SavePlayerName(HttpContext context, string name);
        string GetPlayerName(HttpContext context);

        public void SaveGameStartTime(HttpContext context);
        public DateTime? GetGameStartTime(HttpContext context);
        public void ClearGameStartTime(HttpContext context);
    }
}
