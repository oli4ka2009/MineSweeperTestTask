using MineSweeper.Models;
using System.Collections.Generic;

namespace MineSweeper.Services
{
    public interface IDifficultyAnalyzer
    {
        string GetDifficultyLevel(int width, int height, int mines);
        Dictionary<string, List<GameResult>> GroupResultsByDifficulty(List<GameResult> results, int topCount = 10);
    }
}