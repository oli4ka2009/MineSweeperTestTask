namespace MineSweeper.Models
{
    public enum MoveAction
    {
        Reveal,
        Flag
    }

    public class SolverMove
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public MoveAction Action { get; set; }
    }
}
